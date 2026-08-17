using UnityEngine;

public class NPC : MonoBehaviour
{
    Animator animator;
    [SerializeField] Transform playerTransform;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 playerDir = playerTransform.position - transform.position;

        if (Mathf.Abs(playerDir.x) < Mathf.Abs(playerDir.y))
        {
            animator.SetFloat("XFacing", 0);
            animator.SetFloat("YFacing", -1);
        }
        else
        {
            float x = Mathf.Sign(playerDir.x);
            animator.SetFloat("XFacing", x);
            animator.SetFloat("YFacing", 0);
        }
    }
}
