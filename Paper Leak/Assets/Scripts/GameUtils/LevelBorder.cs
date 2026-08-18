using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorder : MonoBehaviour
{
    [SerializeField] int sceneBuildIndex;
    [Tooltip("The game saves when the player exits the level. This position is assigned to the player's position in the save file.")]
    [SerializeField] Transform exitPlayerPos;

    LevelManager levelManager;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _ = SwitchLevel();
        }
    }

    async Awaitable SwitchLevel()
    {
        Time.timeScale = 0f;
        await FindAnyObjectByType<CartoonEffectManager>().ContractHole();
        Time.timeScale = 1f;

        PlayerController.Instance.GetComponent<PlayerMovement>().SnapToPosition(Vector2Int.RoundToInt(exitPlayerPos.position));
        _ = levelManager.SwitchScene(sceneBuildIndex);
    }
}
