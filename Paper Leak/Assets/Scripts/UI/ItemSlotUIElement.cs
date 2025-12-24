using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUIElement : MonoBehaviour
{
    [SerializeField] Image itemImageRenderer;
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] GameObject selectionPanel;

    public void UpdateSlot(GameObject itemPrefab, int count)
    {
        if (itemPrefab == null)
        {
            ClearSlot();
        }
        else
        {
            SpriteRenderer itemSpriteRenderer = itemPrefab.GetComponentInChildren<SpriteRenderer>();
            SetSlot(itemSpriteRenderer.sprite, itemSpriteRenderer.color, count);
        }
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

    void SetSlot(Sprite itemSprite, Color itemColor, int count)
    {
        itemImageRenderer.enabled = true;

        itemImageRenderer.sprite = itemSprite;
        itemImageRenderer.color = itemColor;
        itemCountText.text = "x" + count.ToString();
    }
}
