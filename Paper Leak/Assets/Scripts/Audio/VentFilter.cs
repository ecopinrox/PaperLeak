using UnityEngine;

public class VentFilter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController _))
        {
            Debug.Log("activate filter");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController _))
        {
            Debug.Log("deactivate filter");
        }
    }
}
