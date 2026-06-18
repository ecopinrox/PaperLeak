using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    [SerializeField] GameObject alertOverlay;
    [SerializeField] Color freezeColor = Color.aliceBlue;

    [SerializeField] Animator guardAnimator;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        alertOverlay.SetActive(false);
    }

    private void FixedUpdate()
    {
        UpdateSpriteOrderInLayer(Mathf.RoundToInt(transform.position.y));
    }

    void Start()
    {
        defaultColor = spriteRenderer.color;
    }

    public void SetCautionOverlayStatus(bool enabled)
    {
        alertOverlay.SetActive(enabled);
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

    void UpdateSpriteOrderInLayer(int yPos)
    {
        spriteRenderer.sortingOrder = -yPos;
    }
}
