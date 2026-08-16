using System;
using System.Threading.Tasks;
using UnityEngine;

public class CutsceneActor : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject alertIndicator;


    void Start()
    {
        animator.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    public void SetState(int? visualState, int? yDestination, float speed, Action onReached)
    {
        Debug.Log($"{transform.position}: {visualState}");
        SetVisualState(visualState);
        _ = Move(yDestination, speed, onReached);
    }

    void SetVisualState(int? visualState)
    {
        if (visualState is not int state) return;

        /*
         * 0123: look UDLR
         * 4: crouch (facing up)
         * 5: freeze (facing up) 
         * 6: freeze and enable alert indicator (facing up)
         */ 

        switch(state)
        {
            case 0:
                animator.speed = 1f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", 0);
                animator.SetFloat("YFacing", 1);
                alertIndicator.SetActive(false);
                break;
            case 1:
                animator.speed = 1f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", 0);
                animator.SetFloat("YFacing", -1);
                alertIndicator.SetActive(false);
                break;
            case 2:
                animator.speed = 1f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", -1);
                animator.SetFloat("YFacing", 0);
                alertIndicator.SetActive(false);
                break;
            case 3:
                animator.speed = 1f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", 1);
                animator.SetFloat("YFacing", 0);
                alertIndicator.SetActive(false);
                break;
            case 4:
                animator.speed = 0f;
                animator.SetBool("IsCrouching", true);
                animator.SetFloat("XFacing", 0);
                animator.SetFloat("YFacing", 1);
                alertIndicator.SetActive(false);
                break;
            case 5:
                animator.speed = 0f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", 0);
                animator.SetFloat("YFacing", 1);
                alertIndicator.SetActive(false);
                break;
            case 6:
                animator.speed = 0f;
                animator.SetBool("IsCrouching", false);
                animator.SetFloat("XFacing", 0);
                animator.SetFloat("YFacing", 1);
                alertIndicator.SetActive(true);
                break;
        }
    }

    async Awaitable Move(int? yDestination, float speed, Action onReachedAction)
    {
        if(yDestination is int yDest)
        {
            animator.SetBool("IsMoving", true);

            while(Math.Abs(yDest - transform.position.y) > 0.001f)
            {
                float yNew = Mathf.MoveTowards(transform.position.y, yDest, speed * Time.fixedDeltaTime);
                transform.position = new Vector3(
                    transform.position.x, 
                    yNew, 
                    transform.position.z
                );

                await Awaitable.FixedUpdateAsync();
            }

            animator.SetBool("IsMoving", false);
        }

        onReachedAction?.Invoke();
    }
}
