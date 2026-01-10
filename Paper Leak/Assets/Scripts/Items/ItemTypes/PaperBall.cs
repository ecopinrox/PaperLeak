using UnityEngine;

public class PaperBall : Item
{
    [SerializeField] float aimRadius = 18f;

    public override async Awaitable<bool> Use()
    {
        Vector2Int? target = await AimingController.Instance.Aim(aimRadius);
        if(target == null)
        {
            Debug.Log("no target");
        }
        else
        {
            Debug.Log("target: " + target);
        }

        return target != null;
    }

    //async Awaitable<Vector2Int?> Aim()
    //{
    //    if (playerController == null)
    //    {
    //        playerController = FindAnyObjectByType<PlayerController>();
    //    }

    //    if (aimingController == null)
    //    {
    //        //aimingController = FindAnyObjectByType<AimingController>();
    //        aimingController = AimingController.Instance;
    //    }

    //    await playerController.SwitchToAimingActionMap(5f);

    //    if(aimingController.AimingState == AimingController.AimState.Finished)
    //    {
    //        return aimingController.selectedPos;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
}
