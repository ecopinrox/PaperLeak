using UnityEngine;

[RequireComponent(typeof(DistractionMovement))]
public class PaperBall : Item
{
    [SerializeField] float aimRadius = 18f;
    [SerializeField] LayerMask targetBlockingMask;
    [SerializeField] LayerMask rayBlockingMask;

    DistractionMovement distractionMovement;

    public override async Awaitable<bool> Use()
    {
        Vector2Int? target = await AimingController.Instance.Aim(aimRadius, targetBlockingMask, rayBlockingMask);
        if (target == null)
        {
            return false;
        }

        PaperBall instance = Instantiate(gameObject, PlayerController.Instance.transform.position, Quaternion.identity).GetComponent<PaperBall>();
        instance.Throw((Vector2Int)target);

        return true;
    }

    private void OnEnable()
    {
        distractionMovement = GetComponent<DistractionMovement>();
    }

    public void Throw(Vector2Int target)
    {
        distractionMovement.SetDestination(target, true);
    }
}
