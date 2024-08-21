﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Path = System.IO.Path;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
namespace obj2mdl_batch_converter
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnLabel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() == ".obj")
                    {
                        if (ObjFileParser.ObjValidator(file) == false) { continue; }
                        ObjFileParser.Parse(file);
                        string targetPath = Path.Combine(Path.GetDirectoryName(file), $"{Path.GetFileNameWithoutExtension(file)}.mdl");
                        ObjFileParser.Save(targetPath);

                    }
                }

            }
        }
    }
    public static class ObjFileParser
    {
        public static List<string> Vertices { get; private set; } = new List<string>();
        public static List<string> Normals { get; private set; } = new List<string>();
        public static List<string> TextureCoordinates { get; private set; } = new List<string>();
        public static List<string> Faces { get; private set; } = new List<string>();
        public static List<int> TriangleVertexIndices { get; private set; } = new List<int>();
        public static void ClearAll() { Vertices.Clear(); Normals.Clear(); TextureCoordinates.Clear(); Faces.Clear(); TriangleVertexIndices.Clear(); }
        public static string Get_Geoset_MDL_String()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("Geoset {")
                .AppendLine($"\tVertices {Vertices.Count} {{");
            foreach (string vertex in Vertices) { stringBuilder.AppendLine(FormatWithCurlyBraces(vertex)); }
            stringBuilder
                .AppendLine("}")
                .AppendLine($"\tNormals {Vertices.Count} {{");
            for (int normalIndex = 0; normalIndex < Vertices.Count; normalIndex++)
            {
                if (normalIndex < Normals.Count) { stringBuilder.AppendLine(FormatWithCurlyBraces(Normals[normalIndex])); }
                else
                {
                    stringBuilder.AppendLine("{ 0, 0, 0 },");
                }
            }
            stringBuilder.AppendLine("}").AppendLine($"\tTVertices {Vertices.Count} {{");
            for (int tVertexIndex = 0; tVertexIndex < Vertices.Count; tVertexIndex++)
            {
                if (tVertexIndex < TextureCoordinates.Count) { stringBuilder.AppendLine(FormatWithCurlyBraces(TextureCoordinates[tVertexIndex])); }
                else
                {
                    stringBuilder.AppendLine("{ 0, 0 },");
                }
            }
            stringBuilder
            .AppendLine("}")
            .AppendLine($"\tVertexGroup {{");
            for (int i = 0; i < Vertices.Count; i++) { stringBuilder.AppendLine("0,"); }
            stringBuilder
            .AppendLine("}")
            .AppendLine($"\tFaces 1 {TriangleVertexIndices.Count} {{")
            .AppendLine($"\t\tTriangles {{")
            .AppendLine($"{{")
            .AppendLine(string.Join(", ", TriangleVertexIndices.ConvertAll(i => i.ToString()).ToArray()))
            .AppendLine($"}},\r\n\t\t}}\r\n\t}}\r\n")
            .AppendLine("\t" + @"Groups 1 1 {
		Matrices { 0 },
	}")
            .AppendLine("MaterialID 0,")
            .AppendLine("SelectionGroup 0,")
            .AppendLine("}");
            return stringBuilder.ToString();
        }

        public static void Parse(string filename)
        {
            if (!File.Exists(filename)) { MessageBox.Show($"The specified file \"{filename}\" does not exist."); return; }

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

            TriangulateFaces();
        }
        private static void TriangulateFaces()
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
                    foreach (string ngon in faceVertices) { indexes.Add(int.Parse(ngon.Split('/')[0]) - 1); }
                    indexes.OrderBy(x => x);
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
        private static bool AnyEmpty() { if (Vertices.Count < 3 || Faces.Count == 0) { return true; } return false; }
        public static void Save(string filename)
        {
            if (AnyEmpty() == true) { MessageBox.Show($"Insufficient geometry at {filename}"); return; }
            string timestamp = DateTime.Now.ToString("dd MMMM yyyy 'at' HH:mm:ss");
            StringBuilder stringBuilder = new StringBuilder()
            .AppendLine($"// Model converted from OBJ to MDL by OBJ2MDL Batch Converter on {timestamp}")
            .AppendLine(MDL_Base)
            .AppendLine(Get_Geoset_MDL_String());
            File.WriteAllText(filename, stringBuilder.ToString());
            ClearAll();
        }
        private const string MDL_Base = @"
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

        private static string FormatWithCurlyBraces(string input)
        {
            if (string.IsNullOrEmpty(input)) { MessageBox.Show("Input string is null or empty."); return ""; }
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            sb.Append("\t\t{ ");
            for (int i = 0; i < parts.Length; i++)
            {
                sb.Append(parts[i]);
                if (i < parts.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" },");
            return sb.ToString();
        }
        //regexes for validation
        //----------------------------------
        private static readonly Regex vertexRegex = new Regex(@"^v\s-?\d+(\.\d+)?([eE][-+]?\d+)?\s-?\d+(\.\d+)?([eE][-+]?\d+)?\s-?\d+(\.\d+)?([eE][-+]?\d+)?$");
        private static readonly Regex vertexNormalRegex = new Regex(@"^vn\s-?\d+(\.\d+)?([eE][-+]?\d+)?\s-?\d+(\.\d+)?([eE][-+]?\d+)?\s-?\d+(\.\d+)?([eE][-+]?\d+)?$");

        private static readonly Regex vertexTextureRegex = new Regex(@"^vt\s-?\d+(\.\d+)?\s-?\d+(\.\d+)?(\s-?\d+(\.\d+)?)?$");
        private static readonly Regex faceRegex = new Regex(@"^f(\s\d+(/\d*)?(/\d*)?)+$");
        private static readonly Regex commentRegex = new Regex(@"^#.*$");
        private static readonly Regex objectRegex = new Regex(@"^o\s+\w+$");
        private static readonly Regex groupRegex = new Regex(@"^g\s+\w+$");
        private static readonly Regex useMaterialRegex = new Regex(@"^usemtl\s+\w+$");
        private static readonly Regex smoothShadingRegex = new Regex(@"^s\s+\w+$");
        private static readonly Regex materialLibRegex = new Regex(@"^mtllib\s+\w+(\.\w+)?$");
        //----------------------------------
        public static bool ObjValidator(string filePath)
        {
            if (!File.Exists(filePath)) { return false; }

            using (StreamReader reader = new StreamReader(filePath))
            {
                int lineNumber = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line) || commentRegex.IsMatch(line))
                    {
                        continue; // Skip empty lines and comments
                    }
                    else if (vertexRegex.IsMatch(line) || vertexTextureRegex.IsMatch(line) ||
                             vertexNormalRegex.IsMatch(line) || faceRegex.IsMatch(line) ||
                             objectRegex.IsMatch(line) || groupRegex.IsMatch(line) ||
                             useMaterialRegex.IsMatch(line) || smoothShadingRegex.IsMatch(line) ||
                             materialLibRegex.IsMatch(line))
                    {
                        continue; // Valid line
                    }
                    else
                    {
                        // Display message box with invalid line information
                        MessageBox.Show($"Invalid line detected at line {lineNumber}: {line}", "Invalid OBJ File");
                        return false;
                    }
                }
            }

            return true; // All lines are valid
        }

    }
}
