using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : MonoBehaviour
{
    public static readonly int COLLISION_CHECK_DELAY_IN_FRAMES = 3;

    [SerializeField] private bool adjustRotation = true;

    private float speed;
    private int damage;
    private Team team;

    private Hitbox hitbox;
    private Vector3 direction;

    private DoEveryXFrame collisionCheck;

    private void Awake() {
        hitbox = GetComponent<Hitbox>();
        collisionCheck = new DoEveryXFrame(COLLISION_CHECK_DELAY_IN_FRAMES, CollisionCheck);
    }

    public void Init(float angleRAD, float speed, int damage, Team team) {
        this.speed = speed;
        this.damage = damage;
        this.team = team;


        float angleDEG = angleRAD * Mathf.Rad2Deg;
        this.direction = new Vector3(Mathf.Cos(angleRAD), Mathf.Sin(angleRAD));
        if (adjustRotation) {
            transform.rotation = Quaternion.Euler(0, 0, angleDEG);
        }
    }

    private void FixedUpdate() {
        
        collisionCheck.UpdateFrame();

        transform.position += Time.fixedDeltaTime * speed * direction;

        if (Physics2D.OverlapCircle(hitbox.GetCenter(), hitbox.GetRadius(), Factory.Instance.walls)) {
            Destroy(this.gameObject);
        }
    }

    public void CollisionCheck() {
        if (team != Team.PLAYER) {
            if (hitbox.Collide(PlayerMovement.Instance.GetPlayerHitbox())) {
                PlayerMovement.Instance.GetEntity().Damage(damage);
            }
        }
        if (team != Team.ENEMY) {
            Hitbox hit = SpacialGrouping.currentGrouping.CollisionWith(hitbox);
            if (hit != null) {
                hit.GetComponent<Entity>()?.Damage(damage);
            }
        }
    }
}

public enum Team { ENEMY, PLAYER, NONE }
