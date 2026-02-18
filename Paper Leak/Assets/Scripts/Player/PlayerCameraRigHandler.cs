using UnityEngine;

public class PlayerCameraRigHandler : MonoBehaviour
{
    [SerializeField] CameraRig defaultRig;
    [SerializeField] LayerMask cameraRegionMask;
    CameraRig currentRig;

    void Start()
    {
        currentRig = defaultRig;
        currentRig.ActivateRig();
    }

    private void FixedUpdate()
    {
        SetActiveCameraRig();
    }

    public void SetActiveCamera(Vector2 input, bool peeking)
    {
        currentRig.SwitchCamera(GetDirection(input), peeking);
    }

    void SetActiveCameraRig()
    {
        CameraRig[] rigs = GetOverlappingRigs(transform.position);
        CameraRig highestPriorityRig = defaultRig;

        foreach (CameraRig rig in rigs)
        {
            if (rig.Priority > highestPriorityRig.Priority)
            {
                highestPriorityRig = rig;
            }
        }

        SwitchRig(highestPriorityRig);
    }

    CameraRig[] GetOverlappingRigs(Vector2 pos)
    {
        Collider2D[] rigColliders = Physics2D.OverlapPointAll(pos, cameraRegionMask);
        CameraRig[] rigs = new CameraRig[rigColliders.Length];

        for (int i = 0; i < rigColliders.Length; i++)
        {
            rigs[i] = rigColliders[i].GetComponent<CameraRegion>().Rig;
        }

        return rigs;
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
        if(newRig == currentRig)
        {
            return;
        }

        newRig.ActivateRig();
        currentRig.DeactivateRig();
        currentRig = newRig;
    }
}
