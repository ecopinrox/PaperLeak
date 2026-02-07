using UnityEngine;

public class AimFOVHandler : MonoBehaviour
{
    MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void RenderAimFOV()
    {
        Debug.Log("render aim FOV");
    }

    public void ClearAimFOV()
    {
        Debug.Log("clear aim FOV");
    }
}
