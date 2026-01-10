using UnityEngine;

public class TestItem : Item
{
    public override async Awaitable<bool> Use()
    {
        await Awaitable.NextFrameAsync();
        Debug.Log("item used");
        return true;
    }
}
