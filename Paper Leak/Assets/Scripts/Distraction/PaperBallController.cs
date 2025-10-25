using UnityEngine;

public class PaperBallController : MonoBehaviour
{
    DistractionMovement movement;

    public void SetDestination(Vector2Int destination, bool isCrawling)
    {
        movement = GetComponent<DistractionMovement>();

        movement.SetDestination(destination, isCrawling);
    }
}
