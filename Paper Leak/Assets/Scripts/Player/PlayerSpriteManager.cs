using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerSpriteManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer mainSR;
    [SerializeField] SpriteRenderer overlaySR;
    [SerializeField] float overlayAlphaValue = 0.5f;

    void Update()
    {
        overlaySR.sprite = mainSR.sprite;
        overlaySR.color = new Color(mainSR.color.r, mainSR.color.g, mainSR.color.b, overlayAlphaValue);
    }
}
