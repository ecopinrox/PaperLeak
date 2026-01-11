using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Landmine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out GuardBrain guardBrain))
        {
            guardBrain.Freeze();
            gameObject.SetActive(false);
        }
    }
}
