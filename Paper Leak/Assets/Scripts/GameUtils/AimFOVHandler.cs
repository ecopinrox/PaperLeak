using UnityEngine;

public class AimFOVHandler : MonoBehaviour
{
    MeshFilter meshFilter;


    [Tooltip("The total number of rays drawn.")][SerializeField] int resolution = 10;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void RenderAimFOV(Vector2 origin, float radius, LayerMask rayBlockingMask)
    {
        Debug.Log("render aim FOV");

        Mesh fovMesh = new();

        Vector3[] vertices  = new Vector3[resolution + 1 ];
        Vector2[] uv        = new Vector2[vertices.Length];
        int[] triangles     = new int[vertices.Length * 3];

        vertices[0] = origin;
        int triangleIndex = 0;
        for(int i = 1; i < vertices.Length; i++)
        {
            float angle = (360f / resolution) * (i - 1);
            Vector2 direction = PolarToRect(angle);
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, radius, rayBlockingMask);
            vertices[i] = (hit) ? hit.point : origin + direction * radius;

            if (i > 1)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i - 1;
                triangles[triangleIndex++] = i;
            }
        }

        triangles[triangleIndex++] = 0;
        triangles[triangleIndex++] = resolution;
        triangles[triangleIndex++] = 1;

        fovMesh.vertices = vertices;
        fovMesh.uv = uv;
        fovMesh.triangles = triangles;

        meshFilter.mesh = fovMesh;
    }

    public void ClearAimFOV()
    {
        Debug.Log("clear aim FOV");

        meshFilter.mesh = null;
    }

    Vector2 PolarToRect(float degrees)
    {
        float rad = Mathf.Deg2Rad * degrees;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
