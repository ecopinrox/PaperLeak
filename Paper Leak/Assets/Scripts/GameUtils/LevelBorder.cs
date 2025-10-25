using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorder : MonoBehaviour
{
    [SerializeField] string levelName;

    LevelManager levelManager;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            levelManager.LoadLevel(levelName);
    }
}
