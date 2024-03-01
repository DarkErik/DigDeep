using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    public GameObject spawnCrackPrefab;

    public EnemyScriptableObj[] enemies;

    public void Awake() {
        Instance = this;
    }

    /// <summary>
    /// Returns the index of a random enemy to spawn, considering all likelyhoods.
    /// </summary>
    /// <returns></returns>
    public int SelectRandomEnemy(int heat) {
        int max = 0;
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i].minHeat <= heat) max += enemies[i].likelyHood;

        int val = Random.Range(1, max + 1);

        int index = 0;
        do {
            if (enemies[index].minHeat > heat) continue;
            val -= enemies[index].likelyHood;
            if (val <= 0) return index;
            index++;
        } while (true);
    }
}

