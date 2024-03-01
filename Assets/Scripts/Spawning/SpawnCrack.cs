using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCrack : MonoBehaviour
{
    private int spawnIndex;
    private Generator heatGen;

    [SerializeField] private int currentHeat;
    [SerializeField] private GameObject enemySpawnEffect;

    public void Init(int spawnIndex, Generator heatGen) {
        this.spawnIndex = spawnIndex;
        this.heatGen = heatGen;
    }

    public void Update() {
        currentHeat += heatGen.Work(Time.deltaTime);
        if (currentHeat >= EnemyFactory.Instance.enemies[spawnIndex].spawnHeat) {
            currentHeat -= EnemyFactory.Instance.enemies[spawnIndex].spawnHeat;
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy() {
        Instantiate(enemySpawnEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Instantiate(EnemyFactory.Instance.enemies[spawnIndex].enemyPrefab, transform.position, Quaternion.identity);
    }
}
