using UnityEngine;

public class PlayerCameraRigHandler : MonoBehaviour
{
    [SerializeField] CameraRig defaultRig;
    CameraRig currentRig;

    void Start()
    {
        currentRig = defaultRig;
        currentRig.ActivateRig();
    }

    public void SetActiveCamera(Vector2 input)
    {
        currentRig.SwitchCamera(GetDirection(input));
    }

    public void SetCameraRig(CameraRig newRig)
    {
        SwitchRig(newRig);
    }

    public void ResetCameraRig(CameraRig rig)
    {
        if (currentRig != rig) 
        {
            return; 
        }
        SwitchRig(defaultRig);
    }

    CameraRig.Direction GetDirection(Vector2 rawDir)
    {
        if (rawDir.x == 0)
        {
            if (rawDir.y == 0) return CameraRig.Direction.O;
            if (rawDir.y > 0) return CameraRig.Direction.N;
            if (rawDir.y < 0) return CameraRig.Direction.S;
        }
        if(rawDir.x > 0)
        {
            if (rawDir.y == 0) return CameraRig.Direction.E;
            if (rawDir.y > 0) return CameraRig.Direction.NE;
            if (rawDir.y < 0) return CameraRig.Direction.SE;
        }
        if(rawDir.x < 0)
        {
            if (rawDir.y == 0) return CameraRig.Direction.W;
            if (rawDir.y > 0) return CameraRig.Direction.NW;
            if (rawDir.y < 0) return CameraRig.Direction.SW;
        }

        return CameraRig.Direction.O;
    }

    void SwitchRig(CameraRig newRig)
    {
        newRig.ActivateRig();
        currentRig.DeactivateRig();
        currentRig = newRig;
    }
}
