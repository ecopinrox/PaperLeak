using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUIElement : MonoBehaviour
{
    [SerializeField] Image itemImageRenderer;
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] GameObject selectionPanel;
    [SerializeField] Image cooldownOverlay;

    Sprite defaultSprite;
    Sprite cooldownSprite;

    public void UpdateSlot(GameObject itemPrefab, int count, bool isInfinite)
    {
        if (itemPrefab == null)
        {
            ClearSlot();
        }
        else
        {
            SpriteRenderer itemSpriteRenderer = itemPrefab.GetComponentInChildren<SpriteRenderer>();
            Item item = itemPrefab.GetComponent<Item>();
            SetSlot(itemSpriteRenderer.sprite, item.CooldownSprite, itemSpriteRenderer.color, count);
            itemCountText.enabled = !isInfinite;
        }
    }

    public void UpdateCooldownOverlay(float fill)
    {
        cooldownOverlay.fillAmount = fill;
        itemImageRenderer.sprite = (fill > Mathf.Epsilon) ? cooldownSprite : defaultSprite;
    }

    public void SelectSlot()
    {
        selectionPanel.SetActive(true);
    }

    public void DeselectSlot()
    {
        selectionPanel.SetActive(false);
    }

    void ClearSlot()
    {
        itemImageRenderer.enabled = false;
        itemCountText.text = null;
    }

    void SetSlot(Sprite itemSprite, Sprite itemCooldownSprite, Color itemColor, int count)
    {
        itemImageRenderer.enabled = true;

        defaultSprite = itemSprite;
        cooldownSprite = itemCooldownSprite;
        itemImageRenderer.color = itemColor;
        itemCountText.text = "x" + count.ToString();
    }
}
