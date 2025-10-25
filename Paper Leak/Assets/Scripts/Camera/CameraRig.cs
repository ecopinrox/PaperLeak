using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraRig : MonoBehaviour
{
    Transform playerTransform = null;

    [Header("Camera settings")]
    [SerializeField] List<CinemachineVirtualCamera> cameras;
    [SerializeField] int cameraSize;
    [SerializeField] int peekingTileOffset = 10;

    [Header("Confiner settings")]
    [SerializeField] PolygonCollider2D confiner;
    [SerializeField] Transform bottomLeftTile;
    [SerializeField] Transform topRightTile;
    [SerializeField] bool centerOnTopRight = false;

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
        if (centerOnTopRight)
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

    public void SwitchCamera(Direction direction)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].Priority = (i == (int)direction) ? 1 : 0;
        }
    }

    public void DeactivateRig()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].Priority = 0;
        }
    }

    public void ActivateRig()
    {
        SwitchCamera(Direction.O);
    }

    void SetFollowTarget()
    {
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.Follow = playerTransform;
        }
    }

    void SetCameraSizes()
    {
        foreach(CinemachineVirtualCamera cam in cameras)
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
            float offsetFraction = (float)peekingTileOffset / cameraSize;

            float posXOffset = defaultX - offsetFraction / aspectRatio;
            float negXOffset = defaultX + offsetFraction / aspectRatio;
            float posYOffset = defaultY + offsetFraction;
            float negYOffset = defaultY - offsetFraction;

            for (int i = 0; i < cameras.Count; i++)
            {
                float xOffset = i switch
                {
                    (int)Direction.NE => posXOffset,
                    (int)Direction.E => posXOffset,
                    (int)Direction.SE => posXOffset,

                    (int)Direction.SW => negXOffset,
                    (int)Direction.W => negXOffset,
                    (int)Direction.NW => negXOffset,

                    _ => defaultX
                };

                float yOffset = i switch
                {
                    (int)Direction.NW => posYOffset,
                    (int)Direction.N => posYOffset,
                    (int)Direction.NE => posYOffset,

                    (int)Direction.SE => negYOffset,
                    (int)Direction.S => negYOffset,
                    (int)Direction.SW => negYOffset,

                    _ => defaultY
                };

                cameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = xOffset;
                cameras[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = yOffset;
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
