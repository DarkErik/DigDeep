using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] private float pullRange = 20f;
    [SerializeField] private float intrestRangeFactor = 1.5f;
    [SerializeField] private float speed;

    [SerializeField] private Animator animator;
    [SerializeField] private Hitbox hitbox;

    private (int, int) spacialGroupingData;

    private State state = State.SLEEP;

    private void Start() {
        spacialGroupingData = SpacialGrouping.currentGrouping.Add(hitbox);
    }

    public void FixedUpdate() {

        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        switch (state) {
            case State.SLEEP:

                if (Vector3.Distance(playerPos, this.transform.position) < pullRange) {
                    state = State.HUNT;
                }

                break;
            case State.HUNT:
                Walk(Time.deltaTime);
                
                if (Vector3.Distance(playerPos, this.transform.position) > intrestRangeFactor) {
                    state = State.SLEEP;
                    animator.SetBool("walk", false);
                }

                break;
        }
    }

    private void Walk(float deltaTime) {
        Vector3 dir = (PlayerMovement.Instance.transform.position - transform.position).normalized;
        transform.position += deltaTime * speed * dir;

        spacialGroupingData = SpacialGrouping.currentGrouping.UpdateHitboxBucket(spacialGroupingData, hitbox);

        if (Mathf.Abs(dir.x) > 0.2f) {
            PlayerMovement.SetDirection(animator.transform, dir.x > 0);
        }
        animator.SetBool("walk", true);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pullRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRange * intrestRangeFactor);
    }


    private enum State { SLEEP, HUNT }
}
