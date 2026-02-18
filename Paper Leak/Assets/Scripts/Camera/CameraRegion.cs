using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class CameraRegion : MonoBehaviour
{
    [field: SerializeField] public CameraRig Rig { get; private set; }
}
