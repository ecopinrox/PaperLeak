using UnityEngine;

public class CutsceneActor : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject alertIndicator;

    void Start()
    {
        alertIndicator.SetActive(false);
        animator.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    public void SetState(int? visualState, int? yDestination, float speed)
    {
        Debug.Log($"actor at pos {transform.position} received the following data: {visualState}, {yDestination}, {speed}");
    }
}
