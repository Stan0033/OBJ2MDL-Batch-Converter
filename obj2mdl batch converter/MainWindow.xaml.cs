using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Path = System.IO.Path;
using System.Text;
using System.Linq;

namespace obj2mdl_batch_converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Man_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    List<string> objFilePaths = new List<string>();

                    foreach (string file in files)
                    {
                        if (System.IO.Path.GetExtension(file).ToLower() == ".obj")
                        {
                            objFilePaths.Add(file);
                        }
                    }

                    if (objFilePaths.Count > 0)
                    {
                        foreach (string path in objFilePaths)
                        {
                            ObjFileParser.Parse(path);
                            string target = ChangeExtension(path, "mdl");
                            ObjFileParser.Save(target);
                        }
                    }

                }
            }
        }

        public static string ChangeExtension(string filename, string newExtension)
        {
            if (!string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(newExtension))
            {
                string directory = Path.GetDirectoryName(filename);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                string newFileName = Path.Combine(directory, $"{fileNameWithoutExtension}.{newExtension}");
                return newFileName;
            }
            else
            {
                throw new ArgumentException("Filename or new extension is null or empty.");
            }
        }
    }
    public static class ObjFileParser
    {
        public static List<string> Vertices { get; private set; }
        public static List<string> Normals { get; private set; }
        public static List<string> TextureCoordinates { get; private set; }
        public static List<string> Faces { get; private set; }
        public static List<int> TriangleVertexIndices = new List<int>();
        private static void Clean()
        {
            Vertices.Clear();
            Normals.Clear();
            TextureCoordinates.Clear();
            Faces.Clear();
            TriangleVertexIndices.Clear();
        }
        static ObjFileParser()
        {
            Vertices = new List<string>();
            Normals = new List<string>();
            TextureCoordinates = new List<string>();
            Faces = new List<string>();
        }
        private static string Get()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Geoset {");
            stringBuilder.AppendLine($"Vertices {Vertices.Count} {{");
            foreach (string s in Vertices) { stringBuilder.AppendLine(FormatWithCurlyBraces(s)); }
            stringBuilder.AppendLine("}");
           
            stringBuilder.AppendLine($"Normals {Vertices.Count} {{");
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (i < Normals.Count) { stringBuilder.AppendLine(FormatWithCurlyBraces(Normals[i])); }
                else
                {
                    stringBuilder.AppendLine("{ 0, 0, 0 },");
                }
            }
           
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine($"TVertices {Vertices.Count} {{");
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (i < TextureCoordinates.Count) { stringBuilder.AppendLine(FormatWithCurlyBraces(TextureCoordinates[i])); }
                else
                {
                    stringBuilder.AppendLine("{ 0, 0 },");
                }
            }
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine($"VertexGroup {{");
            for (int i= 0; i < Vertices.Count; i++)
            {
                stringBuilder.AppendLine("0,");
            }



            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine($"Faces 1 {TriangleVertexIndices.Count} {{");
            stringBuilder.AppendLine($"Triangles {{");
            stringBuilder.AppendLine($"{{");
            stringBuilder.AppendLine(WorkFaces());
         

            stringBuilder.AppendLine($"}},\r\n\t\t}}\r\n\t}}\r\n");





            stringBuilder.AppendLine(@"Groups 1 1 {
		Matrices { 0 },
	}");
            stringBuilder.AppendLine("MaterialID 0,");
            stringBuilder.AppendLine("SelectionGroup 0,");
            stringBuilder.AppendLine("}");


            
            return stringBuilder.ToString();    
        }

        private static string WorkFaces()
        {
            string[] stringArray = TriangleVertexIndices.ConvertAll(i => i.ToString()).ToArray();
            return string.Join(", ", stringArray);
        }

        public static void Parse(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("v "))
                        {
                            Vertices.Add(line.Substring(2).Trim());
                        }
                        else if (line.StartsWith("vn "))
                        {
                            Normals.Add(line.Substring(3).Trim());
                        }
                        else if (line.StartsWith("vt "))
                        {
                            TextureCoordinates.Add(line.Substring(3).Trim());
                        }
                        else if (line.StartsWith("f "))
                        {
                            Faces.Add(line.Substring(2).Trim());
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("The specified file does not exist.");
            }
            TriangulateFaces();
        }
        public static void TriangulateFaces()
        {

            List<int> VertexIndices = new List<int>();
            foreach (var face in Faces)
            {
                string[] faceVertices = face.Split(' ');
                if (faceVertices.Length == 3)
                {
                    foreach (string faceVertex in faceVertices)
                    {
                        int first = int.Parse(faceVertex.Split('/')[0]);
                        VertexIndices.Add(first - 1); ;
                    }
                }
                if (faceVertices.Length == 4)
                {
                    int first = int.Parse(faceVertices[0].Split('/')[0]);
                    int second = int.Parse(faceVertices[1].Split('/')[0]);
                    int third = int.Parse(faceVertices[2].Split('/')[0]);
                    int fourth = int.Parse(faceVertices[3].Split('/')[0]);
                    VertexIndices.Add(first - 1);
                    VertexIndices.Add(second - 1);
                    VertexIndices.Add(third - 1);
                    VertexIndices.Add(first - 1);
                    VertexIndices.Add(third - 1);
                    VertexIndices.Add(fourth - 1);
                }
                if (faceVertices.Length > 4)
                {
                    List<int> indexes = new List<int>();
                    foreach (string ngon in faceVertices) { indexes.Add(int.Parse( ngon.Split('/')[0]) -1); }
                    indexes.OrderBy(x=>x);
                    while (indexes.Count > 2)
                    {
                        TriangleVertexIndices.Add(indexes[0]);
                        TriangleVertexIndices.Add(indexes[1]);
                        TriangleVertexIndices.Add(indexes[2]);
                        indexes.RemoveAt(1);
                    }

                }
                    while (VertexIndices.Count > 2)
                {
                    TriangleVertexIndices.Add(VertexIndices[0]);
                    TriangleVertexIndices.Add(VertexIndices[1]);
                    TriangleVertexIndices.Add(VertexIndices[2]);
                    VertexIndices.RemoveAt(0);
                    VertexIndices.RemoveAt(0);
                    VertexIndices.RemoveAt(0);
                }

            }
        }
        public static void Save(string filename)


        {
           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Model converted from obj to mdl by OBJ2MDL Batch Converter on {DateTime.Now}");
            sb.AppendLine(MDL_Base);
            sb.AppendLine(Get());

            File.WriteAllText(filename, sb.ToString());

            Clean();
        }

        private static string MDL_Base = @"
                Version {
	FormatVersion 800,
}
Model """" {
	NumBones 1,
	NumAttachments 1,
	BlendTime 150,
}
Textures 1 {
	Bitmap {
		Image ""Textures\white.blp"",
	}
}
Materials 1 {
	Material {
		Layer {
			FilterMode None,
            TwoSided,
			static TextureID 0,
		}
	}
}
Bone ""base"" {
	ObjectId 0,
	GeosetId 0,
	GeosetAnimId None,
}
Attachment ""Origin Ref"" {
	ObjectId 1,
	AttachmentID 0,
}
PivotPoints 2 {
	{ 0, 0, 0 },
	{ 0, 0, 0 },
}
Sequences 2 {
	Anim ""Stand"" {
		Interval { 0, 999 },
}
Anim ""Death"" {
		Interval { 1000, 1999 },
}
}
"
;
        public static string FormatWithCurlyBraces(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                sb.Append( "{ ");

                for (int i = 0; i < parts.Length; i++)
                {
                    sb.Append(parts[i]);
                    if (i < parts.Length - 1)
                    {
                        sb.Append( ", ");
                    }
                }

                sb.Append(" },");
                return sb.ToString();
            }
            else
            {
                throw new ArgumentException("Input string is null or empty.");
            }
        }
    }

}
