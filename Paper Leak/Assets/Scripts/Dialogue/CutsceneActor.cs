using System;
using System.Threading.Tasks;
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

    public void SetState(int? visualState, int? yDestination, float speed, Action onReached)
    {
        Debug.Log($"actor at pos {transform.position} received visual state {visualState}");

        _ = Move(yDestination, speed, onReached);
    }

    async Awaitable Move(int? yDestination, float speed, Action onReachedAction)
    {
        if(yDestination is int yDest)
        {
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
        }

        onReachedAction?.Invoke();
    }
}
