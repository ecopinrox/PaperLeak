using UnityEngine;
using static UIManager;

public class AimingController : MonoBehaviour
{

    [SerializeField] float slowTimeFactor = 0.6f;
    [SerializeField] GameObject tileHighlight;

    GridManager gridManager;
    UIManager uiManager;

    bool isAiming = false;

    public Vector2Int? SelectedPos { get { return tileHighlight.activeSelf ? Vector2Int.RoundToInt(tileHighlight.transform.position) : null; } }

    private void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        uiManager = gridManager.GetComponent<UIManager>();
    }

    bool IsLocationMarkable(Vector2 pos) => gridManager.IsWalkable(Vector2Int.RoundToInt(pos));

    private void Update()
    {
        if (!isAiming) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (IsLocationMarkable(mousePosition))
        {
            tileHighlight.SetActive(true);
            tileHighlight.transform.position = (Vector2)Vector2Int.RoundToInt(mousePosition);
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    public void EnterAimMode()
    {
        isAiming = true;
        Time.timeScale = slowTimeFactor;
        uiManager.SetAimModePanelStatus(true);
    }

    public void ExitAimMode()
    {
        isAiming = false;
        Time.timeScale = 1f;
        uiManager.SetAimModePanelStatus(false);
        tileHighlight.SetActive(false);
    }
}
