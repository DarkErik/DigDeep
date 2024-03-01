using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotWeapon : BasicWeapon
{
    [SerializeField] private int shotAmount = 3;
    [SerializeField] private float betweenShotTime = 0.2f;

    public override void Shoot(float angleRAD, Vector3 aimPosition, Team team) {
        StartCoroutine(SpawnShots(angleRAD));
    }

    private IEnumerator SpawnShots(float angleRAD) {
        for(int i = 0; i < shotAmount; i++) {
            SpawnShot(angleRAD);
            yield return new WaitForSeconds(betweenShotTime);
        }
    }
}
