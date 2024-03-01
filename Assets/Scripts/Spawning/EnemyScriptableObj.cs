using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyScriptObj", menuName = "Enemy obj")]
public class EnemyScriptableObj : ScriptableObject {
    public GameObject enemyPrefab;
    public int minHeat;
    public int spawnHeat;
    public int likelyHood;
}
