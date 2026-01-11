using UnityEngine;

public class LandmineSetter : Item
{
    [SerializeField] GameObject landminePrefab;
    [SerializeField] float radius;
    [SerializeField] LayerMask blockingMask;

    public async override Awaitable<bool> Use()
    {
        Vector2Int? target = await AimingController.Instance.Aim(radius, blockingMask, blockingMask);

        if(target == null)
        {
            return false;
        }

        Instantiate(landminePrefab, (Vector2)target, Quaternion.identity);
        return true;
    }
}
