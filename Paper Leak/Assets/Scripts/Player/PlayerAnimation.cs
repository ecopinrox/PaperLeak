using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator mainAnimator;
    [SerializeField] Animator overlayAnimator;

    public void SetDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
        {
            return;
        }

        mainAnimator.SetFloat("XFacing", direction.x);
        mainAnimator.SetFloat("YFacing", direction.y);
    }

    public void SetMoving(bool isMoving)
    {
        mainAnimator.SetBool("IsMoving", isMoving);
    }

    public void SetCrouching(bool isCrouching)
    {
        mainAnimator.SetBool("IsCrouching", isCrouching);
    }
}
