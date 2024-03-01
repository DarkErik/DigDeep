using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSpawner : MonoBehaviour {
    [SerializeField] private int heat = 10;
    private int currentlyUsedHeat;

    private List<Vector3> spawnerGroups = new List<Vector3>();

    [SerializeField] private int spawnerHeatCost = 5;
    [SerializeField] private Generator heatGen;

    public void Start() {
    }

    public void Update() {
        heat += heatGen.Work(Time.deltaTime);
        if (currentlyUsedHeat < heat) {
            currentlyUsedHeat += spawnerHeatCost;
            try {
                AddSpawner();
            } catch (GenerationFailedException) { }
        }
    }

    private Vector3 SelectSpawnPoint() {
        Vector3 retValue;
        if (Random.value > 0.5f && spawnerGroups.Count > 0) { // Get Near an already existing patch
            retValue = Util.TryXTimes(50, () => {
                    LevelDataObject lvlData = TileMapManager.Instance.LevelData;

                    int index = Random.Range(0, spawnerGroups.Count);
                    Vector3 pos = new Vector3(spawnerGroups[index].x + Random.Range(-5, 5), spawnerGroups[index].y + Random.Range(-5, 5));
                    return pos;
                },
                (Vector3 pos) => !TileMapManager.Instance.LevelData.IsTile(pos, TileType.GROUND)
            );
        } else {
            retValue = Util.TryXTimes(50, () => {
                LevelDataObject lvlData = TileMapManager.Instance.LevelData;
                Vector3 pos = new Vector3(Random.Range(0, lvlData.width), Random.Range(0, lvlData.height));
                return pos;
            },
                (Vector3 pos) => !TileMapManager.Instance.LevelData.IsTile(pos, TileType.GROUND)
            );
            spawnerGroups.Add(retValue);
        }
        return retValue + new Vector3(.5f, .5f);
    }
    private void AddSpawner() {
        Vector3 spawnPoint = SelectSpawnPoint();

        SpawnCrack crack = Instantiate(EnemyFactory.Instance.spawnCrackPrefab, spawnPoint, Quaternion.identity).GetComponent<SpawnCrack>();
        crack.Init(EnemyFactory.Instance.SelectRandomEnemy(heat), new Generator(5, 20));
    }

    
}


