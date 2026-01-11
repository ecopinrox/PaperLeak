using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DistractionMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask crawlingMask;

    public event Action OnReached;

    Collider2D selfCollider;

    bool isStopped;
    bool canPassUnderCrawlableAreas = false;

    private void OnEnable()
    {
        selfCollider = GetComponent<Collider2D>();
    }

    public void SetDestination(Vector2Int destination, bool canPassUnderCrawlableAreas)
    {
        this.canPassUnderCrawlableAreas = canPassUnderCrawlableAreas;
        StartCoroutine(MoveCoroutine(destination)); 
    }

    IEnumerator MoveCoroutine(Vector2Int destination)
    {
        const float tolerance = 0.01f;

        while (!isStopped && Vector2.Distance(transform.position, destination) > tolerance)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isStopped = true;
        OnReached?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!isStopped && collider.TryGetComponent(out GuardBrain _))
        {
            isStopped = true;
            DisableCollider();
        }
        else if (!canPassUnderCrawlableAreas  || (crawlingMask.value & 1 << collider.gameObject.layer) == 0)
        {
            isStopped = true;
        }
    }

    void DisableCollider()
    {
        selfCollider.enabled = false;
    }
}
