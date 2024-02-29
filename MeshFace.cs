using Godot;



[Tool]
public partial class MeshFace : MeshInstance3D
{
    [Export]
    public float Radius
    {
        get => _size;
        set
        {
            _size = value;
            UpdateMesh();
        }
    }

    private float _size = 1;


    [Export]
    public int Subdivide
    {
        get => _subdivide;
        set
        {
            _subdivide = value;
            UpdateMesh();
        }
    }

    private int _subdivide = 10;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())  // 如果不在编辑器中，就不需要更新模型
        {
            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        var originalMesh = new BoxMesh()
        {
            Size = new Vector3(_size, _size, _size),
            SubdivideWidth = _subdivide,
            SubdivideHeight = _subdivide,
            SubdivideDepth = _subdivide,
            // ... 可以设置其他属性 ...
        };
        Mesh = originalMesh;
        // 将BoxMesh转换为ArrayMesh
        var arrayMesh = new ArrayMesh();
        originalMesh.GenerateTriangleMesh();

        // 获取所有的顶点、法线、UV坐标和索引
        var arrays = originalMesh.SurfaceGetArrays(0);
        var vertices = (Vector3[])arrays[(int)ArrayMesh.ArrayType.Vertex];
        var normals = (Vector3[])arrays[(int)ArrayMesh.ArrayType.Normal];
        var uvs = (Vector2[])arrays[(int)ArrayMesh.ArrayType.TexUV];
        var indices = (int[])arrays[(int)ArrayMesh.ArrayType.Index];

        // 修改所有的顶点位置
        GD.Print("vertices : ", vertices.Length, " normals : ", normals.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            //Vector3 vert = vertices[i];
            //float x2 = vert.X * vert.X;
            //float y2 = vert.Y * vert.Y;
            //float z2 = vert.Z * vert.Z;

            //float newX = vert.X * (float)Math.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
            //float newY = vert.Y * (float)Math.Sqrt(1 - (z2 + x2) / 2 + (z2 * x2) / 3);
            //float newZ = vert.Z * (float)Math.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);
            //vertices[i] = new Vector3(newX, newY, newZ).Normalized() * _size;

            vertices[i] = vertices[i].Normalized()*_size;
            normals[i] = vertices[i].Normalized();
        }

        // 创建新的ArrayMesh并添加修改后的顶点、法线、UV坐标和索引
        var newArrays = new Godot.Collections.Array();
        newArrays.Resize((int)ArrayMesh.ArrayType.Max);
        newArrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
        newArrays[(int)ArrayMesh.ArrayType.Normal] = normals;
        newArrays[(int)ArrayMesh.ArrayType.TexUV] = uvs;
        newArrays[(int)ArrayMesh.ArrayType.Index] = indices;
        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, newArrays);

        // 将修改后的ArrayMesh设置回MeshInstance3D节点
        this.Mesh = arrayMesh;
        //this.Mesh = originalMesh;
    }
    
}
