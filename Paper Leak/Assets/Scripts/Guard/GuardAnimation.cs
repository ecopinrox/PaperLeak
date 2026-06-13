using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    [SerializeField] Color cautionColor = Color.yellow;
    [SerializeField] Color freezeColor = Color.aliceBlue;

    [SerializeField] Animator guardAnimator;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        defaultColor = spriteRenderer.color;
    }

    public void ChangeToCautionColor()
    {
        spriteRenderer.color = cautionColor;
    }

    public void ChangeToIdleColor()
    {
        spriteRenderer.color = defaultColor;
    }

    public void ChangeToFreezeColor()
    {
        spriteRenderer.color = freezeColor;
    }

    public void LookInDirection(Vector2 direction)
    {
        guardAnimator.SetFloat("XFacing", direction.x);
        guardAnimator.SetFloat("YFacing", direction.y);
    }

    public void SetMoving(bool moving)
    {
        guardAnimator.SetBool("IsMoving", moving);
    }

    public void SetCrouching(bool crouching)
    {
        guardAnimator.SetBool("IsCrouching", crouching);
    }
}
