using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject mainSprite;

    SpriteRenderer mainSpriteRenderer;
    Animator mainAnimator;

    void Awake()
    {
        mainSpriteRenderer = mainSprite.GetComponent<SpriteRenderer>();
        mainAnimator = mainSprite.GetComponent<Animator>();
    }

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
        mainSpriteRenderer.sortingLayerName = isCrouching ? "CharacterCrouching" : "Character";
        mainAnimator.SetBool("IsCrouching", isCrouching);
    }
}
