using UnityEngine;

public class Book : Item
{
    [SerializeField] GameObject paperBallPrefab;
    [SerializeField] int paperBallAdditionCount = 3;

    [Header("Paper Tearing Sound")]
    [SerializeField] GameObject pointSoundDistractionPrefab;
    [SerializeField] SoundData paperTearingSFX;

    public async override Awaitable<bool> Use()
    {
        PlayerInventory.Instance.AddItems(paperBallPrefab, paperBallAdditionCount);

        //instantiate sound distraction
        SoundDistraction soundInstance = Instantiate(pointSoundDistractionPrefab, PlayerController.Instance.transform.position, Quaternion.identity).GetComponent<SoundDistraction>();
        soundInstance.PlaySound(paperTearingSFX);

        return true;
    }
}
