using UnityEngine;

public class ChallengeUI : MonoBehaviour
{
    [SerializeField] GameObject tickImage;

    public void SetStatus(bool complete)
    {
        tickImage.SetActive(complete);
    }
}
