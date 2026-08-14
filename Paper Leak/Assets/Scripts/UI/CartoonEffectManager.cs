using UnityEngine;
using System.Threading.Tasks;

public class CartoonEffectManager : MonoBehaviour
{
    [SerializeField] GameObject mask;
    [SerializeField] SpriteMask hole;
    [SerializeField] float outerPhaseScale = 70f;
    [SerializeField] float innerPhaseScale = 3f;
    [SerializeField] float outerPhaseTime = 3f;
    [SerializeField] float outerHoldTime = 3f;
    [SerializeField] float innerPhaseTime = 1f;
    [SerializeField] float innerHoldTime = 0.5f;

    private void Start()
    {
        mask.SetActive(true);
        _ = ExpandHole();
    }

    private void FixedUpdate()
    {
        hole.transform.position = PlayerController.Instance.transform.position;
    }

    async Awaitable ExpandHole()
    {
        hole.transform.localScale = Vector3.zero;
        await Task.Delay((int)(1000 * innerHoldTime));
        await LerpScale(innerPhaseScale, innerPhaseTime);
        await Task.Delay((int)(1000 * outerHoldTime));
        await LerpScale(outerPhaseScale, outerPhaseTime);
        hole.transform.localScale = Vector3.one * outerPhaseScale;
    }
    
    public async Awaitable ContractHole()
    {
        await LerpScale(innerPhaseScale, outerPhaseTime);
        await Task.Delay((int)(1000 * outerHoldTime));
        await LerpScale(0, innerPhaseTime);
        hole.transform.localScale = Vector3.zero;
    }

    async Awaitable LerpScale(float end, float duration)
    {
        float start = hole.transform.localScale.x;

        for (float time = 0; time < duration; await Task.Delay((int)(1000 * Time.unscaledDeltaTime)), time += Time.unscaledDeltaTime)
        {
            float value = time / duration;
            hole.transform.localScale = Vector3.one * Mathf.Lerp(start, end, value); 
        }
    }
}