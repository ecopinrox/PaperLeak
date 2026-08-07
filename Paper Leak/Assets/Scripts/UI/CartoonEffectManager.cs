using UnityEngine;

public class CartoonEffectManager : MonoBehaviour
{
    [SerializeField] SpriteMask hole;

    private void FixedUpdate()
    {
        hole.transform.position = PlayerController.Instance.transform.position;
    }
}
