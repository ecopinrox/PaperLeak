using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpriteManager : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    [SerializeField] Color cautionColor = Color.yellow;
    [SerializeField] Color crouchColor = Color.green;
    [SerializeField] Color freezeColor = Color.aliceBlue;

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

    public void ChangeToCrouchColor()
    {
        spriteRenderer.color = crouchColor;
    }

    public void ChangeToFreezeColor()
    {
        spriteRenderer.color = freezeColor;
    }
}
