using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] LayerMask crawlingMask;

    public event Action OnReached;

    bool isStopped;
    bool canPassUnderCrawlableAreas = false;

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
        if(!isStopped && collider.TryGetComponent(out GuardBrain guard))
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
        GetComponent<Collider2D>().enabled = false;
    }
}
