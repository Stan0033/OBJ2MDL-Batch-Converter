using System.Collections.Generic;
using System.Linq;
namespace obj2mdl_batch_converter
{
    public class RawObject
    {
        public string Name = string.Empty;
        public List<string> Vertices { get; private set; } = new List<string>();
        public List<string> Normals { get; private set; } = new List<string>();
        public List<string> TextureCoordinates { get; private set; } = new List<string>();
        public List<string> Faces { get; private set; } = new List<string>();
        public List<int> TriangleVertexIndices { get;  set; } = new List<int>();
        public RawObject(string name) { Name = name; }
        public RawObject Clone()
        {
            RawObject obj = new RawObject(Name);
            obj.Vertices = Vertices.ToList();
            obj.Normals = Normals.ToList();
            obj.TextureCoordinates = TextureCoordinates.ToList();
            obj.Faces = Faces.ToList();
            obj.TriangleVertexIndices = TriangleVertexIndices.ToList();
            return obj;
        }
    }
}
