using Cinemachine;
using System;
using UnityEngine;

[ExecuteAlways]
public class CameraRig : MonoBehaviour
{
    Transform playerTransform = null;

    [Header("Camera settings")]
    [SerializeField] int cameraSize;
    [SerializeField] CinemachineVirtualCamera originCamera;

    /// <summary>
    /// Movement cameras are in the order N, E, S, W.
    /// </summary>
    [SerializeField] CinemachineVirtualCamera[] movementCameras;
    [SerializeField] int lookaheadTileOffset = 4;

    /// <summary>
    /// SetPeekingCamera cameras are in the order N, NE, E, SE, S, SW, W, NW. 
    /// </summary>
    [SerializeField] CinemachineVirtualCamera[] peekCameras;
    [SerializeField] int peekingTileOffset = 10;

    [Header("Confiner settings")]
    [SerializeField] PolygonCollider2D confiner;
    [SerializeField] Transform bottomLeftTile;
    [SerializeField] Transform topRightTile;
    [SerializeField] bool pivotOnTopRight = false;

    static readonly float aspectRatio = 1.25f;

    public enum Direction
    {
        O, N, NE, E, SE, S, SW, W, NW 
    }

    private void Awake()
    {
        if(playerTransform == null)
        {
            playerTransform = FindAnyObjectByType<PlayerCameraRigHandler>().transform;
            SetFollowTarget();
        }
    }

    private void OnValidate()
    {
        SetCameraSizes();
        SetCameraOffsets();
    }

    public void SetConfinerBounds()
    {
        if (pivotOnTopRight)
        {
            Vector2 adjustedBottomLeftCorner = bottomLeftTile.position;
            if(topRightTile.position.x - bottomLeftTile.position.x < cameraSize - 1)
            {
                adjustedBottomLeftCorner.x = topRightTile.position.x - cameraSize + 1;
            }
            if(topRightTile.position.y - bottomLeftTile.position.y < cameraSize - 1)
            {
                adjustedBottomLeftCorner.y = topRightTile.position.y - cameraSize + 1;
            }
            bottomLeftTile.position = adjustedBottomLeftCorner;
        }
        else
        {
            Vector2 adjustedTopRightCorner = topRightTile.position;
            if(topRightTile.position.x - bottomLeftTile.position.x < cameraSize - 1)
            {
                adjustedTopRightCorner.x = cameraSize + bottomLeftTile.position.x - 1;
            }
            if(topRightTile.position.y - bottomLeftTile.position.y < cameraSize - 1)
            {
                adjustedTopRightCorner.y = cameraSize + bottomLeftTile.position.y - 1;
            }
            topRightTile.position = adjustedTopRightCorner;
        }

        try
        {
            float rightSideOffset = cameraSize * (aspectRatio - 1);

            Vector2 bottomLeft = new(bottomLeftTile.position.x - 0.5f, bottomLeftTile.position.y - 0.5f);
            Vector2 topRight = new(topRightTile.position.x + 0.5f, topRightTile.position.y + 0.5f);
            Vector2 bottomRight = new(topRight.x, bottomLeft.y);
            Vector2 topLeft = new(bottomLeft.x, topRight.y);

            bottomRight += new Vector2(rightSideOffset, 0);
            topRight += new Vector2(rightSideOffset, 0);
            
            confiner.points = new Vector2[] { bottomLeft, bottomRight, topRight, topLeft };
        }
        catch(System.Exception)
        {

        }
    }

    public void SwitchCamera(Direction direction, bool peeking)
    {
        originCamera.Priority = (direction == Direction.O) ? 1 : 0;

        for(int i = 0; i < movementCameras.Length; i++)
        {
            movementCameras[i].Priority = (!peeking && (2 * i + 1) == (int)direction) ? 1 : 0;
        }

        for (int i = 0; i < peekCameras.Length; i++)
        {
            peekCameras[i].Priority = (peeking && (i + 1) == (int)direction) ? 1 : 0;
        }
    }

    public void DeactivateRig()
    {
        originCamera.Priority = 0;

        foreach(CinemachineVirtualCamera cam in movementCameras)
        {
            cam.Priority = 0;
        }

        foreach(CinemachineVirtualCamera cam in peekCameras)
        {
            cam.Priority = 0;
        }
    }

    public void ActivateRig()
    {
        SwitchCamera(Direction.O, false);
    }

    void SetFollowTarget()
    {
        originCamera.Follow = playerTransform;

        foreach (CinemachineVirtualCamera cam in movementCameras)
        {
            cam.Follow = playerTransform;
        }

        foreach (CinemachineVirtualCamera cam in peekCameras)
        {
            cam.Follow = playerTransform;
        }
    }

    void SetCameraSizes()
    {
        originCamera.m_Lens.OrthographicSize = cameraSize / 2f;

        foreach(CinemachineVirtualCamera cam in movementCameras)
        {
            cam.m_Lens.OrthographicSize = cameraSize / 2f;
        }

        foreach(CinemachineVirtualCamera cam in peekCameras)
        {
            cam.m_Lens.OrthographicSize = cameraSize / 2f;
        }
    }

    void SetCameraOffsets()
    {
        float defaultY = 0.5f;
        float defaultX = 0.5f * (1 / aspectRatio);

        try
        {
            //origin offsets
            originCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = defaultX;
            originCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = defaultY;

            //lookahead offsets
            float lookaheadOffsetFraction = (float)lookaheadTileOffset / cameraSize;
            float posXLookOffset = defaultX - lookaheadOffsetFraction / aspectRatio;
            float negXLookOffset = defaultX + lookaheadOffsetFraction / aspectRatio;
            float posYLookOffset = defaultY + lookaheadOffsetFraction;
            float negYLookOffset = defaultY - lookaheadOffsetFraction;

            for(int i = 0; i < movementCameras.Length; i++)
            {
                float xLookOffset = (2 * i + 1) switch
                {
                    (int)Direction.E => posXLookOffset,
                    (int)Direction.W => negXLookOffset,
                    _ => defaultX
                };

                float yLookOffset = (2 * i + 1) switch
                {
                    (int)Direction.N => posYLookOffset,
                    (int)Direction.S => negYLookOffset,
                    _ => defaultY
                };

                movementCameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = xLookOffset;
                movementCameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = yLookOffset;
            }

            //peek offsets
            float peekOffsetFraction = (float)peekingTileOffset / cameraSize;
            float posXPeekOffset = defaultX - peekOffsetFraction / aspectRatio;
            float negXPeekOffset = defaultX + peekOffsetFraction / aspectRatio;
            float posYPeekOffset = defaultY + peekOffsetFraction;
            float negYPeekOffset = defaultY - peekOffsetFraction;

            for (int i = 0; i < peekCameras.Length; i++)
            {
                float xPeekOffset = (i + 1) switch
                {
                    (int)Direction.NE   => posXPeekOffset,
                    (int)Direction.E    => posXPeekOffset,
                    (int)Direction.SE   => posXPeekOffset,

                    (int)Direction.SW   => negXPeekOffset,
                    (int)Direction.W    => negXPeekOffset,
                    (int)Direction.NW   => negXPeekOffset,

                    _ => defaultX
                };

                float yPeekOffset = (i + 1) switch
                {
                    (int)Direction.NW   => posYPeekOffset,
                    (int)Direction.N    => posYPeekOffset,
                    (int)Direction.NE   => posYPeekOffset,

                    (int)Direction.SE   => negYPeekOffset,
                    (int)Direction.S    => negYPeekOffset,
                    (int)Direction.SW   => negYPeekOffset,

                    _ => defaultY
                };

                peekCameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = xPeekOffset;
                peekCameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = yPeekOffset;
            }
        }
        catch (NullReferenceException)
        {
            //this code block frequently produces NullReferenceExceptions but this could just be due to the code running before the cameras are even properly loaded in
            //anyway it's of no consequence so the error can be discarded
        } 
        catch (IndexOutOfRangeException)
        { 
            //same as above
        }
    }
}
