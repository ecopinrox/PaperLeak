using UnityEngine;

public class Artifact : Item
{
    public async override Awaitable<bool> Use()
    {
        Transform playerTransform = PlayerController.Instance.transform;
        Awaitable s;

        return true;
    }
}
