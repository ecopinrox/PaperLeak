using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class RoomFocusedCamera : MonoBehaviour
{
    [SerializeField] CameraRig cameraRig;

    private void OnTriggerEnter2D(Collider2D collision)
    {            
        if(collision.gameObject.TryGetComponent(out PlayerCameraRigHandler playerCameraRigHandler))
        {
            playerCameraRigHandler.SetCameraRig(cameraRig);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerCameraRigHandler playerCameraRigHandler))
        {
            playerCameraRigHandler.ResetCameraRig(cameraRig);
        }
    }
}
