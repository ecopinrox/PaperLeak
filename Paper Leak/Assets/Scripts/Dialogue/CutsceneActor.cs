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

    public void SetState(string state)
    {
        Debug.Log($"actor {gameObject.name} (at {transform.position}) state set to {state}");
    }
}
