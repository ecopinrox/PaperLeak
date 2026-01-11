using UnityEngine;

public class Artifact : Item
{
    [Header("Aiming")]
    [SerializeField] float radius = 10f;
    [SerializeField] LayerMask targetBlockingMask;
    [SerializeField] LayerMask rayBlockingMask;

    [Header("Teleport Sound")]
    [SerializeField] GameObject soundDistractionPrefab;
    [SerializeField] SoundData teleportSFX;

    public async override Awaitable<bool> Use()
    {
        Transform playerTransform = PlayerController.Instance.transform;
        PlayerMovement playerMovement = PlayerController.Instance.GetComponent<PlayerMovement>();

        Vector2Int? target = await AimingController.Instance.Aim(radius, targetBlockingMask, rayBlockingMask);
        if (target == null)
        {
            return false;
        }

        playerMovement.SnapToPosition((Vector2Int)target);

        SoundDistraction soundDistractionInstance = Instantiate(soundDistractionPrefab, playerTransform.position, Quaternion.identity).GetComponent<SoundDistraction>();
        soundDistractionInstance.PlaySound(teleportSFX);

        return true;
    }
}
