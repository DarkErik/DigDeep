using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Drilling")]
    [SerializeField] private float drillOffset = 0.5f;
    [SerializeField] private float drillRadius = 0.5f;
    [SerializeField] private float drillDamagePerSecond = 1f;

    [Header("Entity stuff")]
    [SerializeField] private Entity entity;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float invizDuration = 0.5f;
    private float currentInviz = 0f;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private BasicWeapon weapon;

    [Header("Animation")]
    [SerializeField] private Transform animationBase;
    private Animator anim;

    private Vector3 lastMovementAxisInput;

    public void Awake() {
        
        Instance = this;
        anim = animationBase.GetComponent<Animator>();
    }

    public void FixedUpdate() {
        UpdatePlayerMovement();
        UpdateDrill();
        CheckCollisionWithEnemies();
    }

    public void Update() {
        weapon.PullTrigger();
    }

    private void CheckCollisionWithEnemies() {
        if(currentInviz <= 0) {
            Hitbox collider = SpacialGrouping.currentGrouping.CollisionWith(hitbox);
            if (collider != null) {
                Entity ent = collider.GetComponent<Entity>();
                if (ent != null) {
                    entity.Damage(ent.GetTouchDamage());
                    //Debug.Log("PlayerHit!");
                    currentInviz = invizDuration;
                }
            }
        } else {
            currentInviz -= Time.fixedDeltaTime;
        }
    }

    private void UpdatePlayerMovement() {
        lastMovementAxisInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (lastMovementAxisInput != Vector3.zero) {
            SetDirection(animationBase, lastMovementAxisInput.x > 0);
            transform.position += lastMovementAxisInput * speed * Time.fixedDeltaTime;
            anim.SetBool("walk", true);
        } else {
            anim.SetBool("walk", false);
        }

    }

    private void UpdateDrill() {
        if (!Input.GetKey(KeyCode.Space))
            return;

        Vector2 drillPos = transform.position + lastMovementAxisInput * drillOffset;

        TileMapManager.Instance.Drill(drillPos, drillRadius, Time.fixedDeltaTime * drillDamagePerSecond);
    }

    public Entity GetEntity() {
        return entity;
    }

    public Hitbox GetPlayerHitbox() {
        return hitbox;
    }

    public static void SetDirection(Transform trans, bool right) {
        trans.localRotation = Quaternion.Euler(0, right ? 0 : 180, trans.localRotation.eulerAngles.z);
    }



    private void OnDrawGizmos() {
        Vector3 offs = Vector3.right * drillOffset;

        if (lastMovementAxisInput != Vector3.zero) {
            offs = lastMovementAxisInput * drillOffset;
        }

        Gizmos.DrawLine(transform.position, transform.position + offs);
        Gizmos.DrawWireSphere(transform.position + offs, drillRadius);
    }
}
