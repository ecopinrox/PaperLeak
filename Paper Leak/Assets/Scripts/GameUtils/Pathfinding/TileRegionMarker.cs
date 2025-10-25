using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileRegionMarker : MonoBehaviour
{
    [SerializeField] GameObject textObject;
    readonly Dictionary<Vector2Int, GameObject> tileRegionTexts = new();

    public void MarkTile(Vector2Int pos, int region)
    {
        if (!tileRegionTexts.ContainsKey(pos))
        {
            GameObject instance = Instantiate(textObject, (Vector2)pos, Quaternion.identity, transform);
            tileRegionTexts.Add(pos, instance);
        }

        TextMeshPro regionText = tileRegionTexts[pos].GetComponent<TextMeshPro>();
        regionText.text = region.ToString();
    }
}
