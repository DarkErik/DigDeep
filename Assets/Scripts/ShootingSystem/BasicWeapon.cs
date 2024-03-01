using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicWeapon : MonoBehaviour
{
    private static readonly float MAX_AIM_RADIUS_NEAREST_ENEMY = 7.5f;

    [SerializeField] private float spread = 5;
    [SerializeField] private float shotSpeed = 10;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private int shotDamage = 20;
    [SerializeField] private AimTarget target;
    [SerializeField] private Team team = Team.PLAYER;
    [SerializeField] private Transform shootingPos;
    [SerializeField] private GameObject shotPrefab;

    private float currentReloadTimer;

    protected virtual void Update() {
        if (currentReloadTimer > 0) {
            currentReloadTimer -= currentReloadTimer;
        }
    }

    public bool PullTrigger() {
        if (currentReloadTimer <= 0) {
            Vector3 aimPosition = Vector3.zero;

            switch (target) {
                case AimTarget.NEAREST_ENEMY:
                    Hitbox hit = SpacialGrouping.currentGrouping.GetClosestHitbox(PlayerMovement.Instance.transform.position, MAX_AIM_RADIUS_NEAREST_ENEMY);
                    if (hit == null) return false;
                    aimPosition = hit.GetCenter();

                    break;
                case AimTarget.PLAYER:
                    aimPosition = PlayerMovement.Instance.transform.position;
                    break;
            }

            Vector3 dir = (aimPosition - transform.position).normalized;

            Shoot(Mathf.Atan2(dir.y, dir.x), aimPosition, team);
            currentReloadTimer = reloadTime;
            return true;
        }
        return false;
    }

    public abstract void Shoot(float angleRAD, Vector3 aimPosition, Team team);

    protected void SpawnShot(float angleRAD) {
        Instantiate(shotPrefab, shootingPos.position, Quaternion.identity).GetComponent<BasicShot>().Init(angleRAD + Random.Range(-spread / 2, spread / 2) * Mathf.Deg2Rad, shotSpeed, shotDamage, team);
    }

    public enum AimTarget { NEAREST_ENEMY, PLAYER }

}
