using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private int maxHp = 50;
    private int hp;

    [SerializeField] private int touchDamage = 10;

    public delegate void OnDeath(Entity entity);

    private OnDeath onDeath;

    public void Awake() {
        hp = maxHp;
    }

    public void AddOnDeathEvent(OnDeath func) {
        onDeath += func;
    }

    public void Damage(int amount) {
        hp -= amount;
        //Debug.Log("HP: " + hp);
        if (hp <= 0) {
            if (onDeath != null)onDeath(this);
            
            Destroy(this.gameObject);
            return;
        }
    }

    public int GetTouchDamage() {
        return touchDamage;
    }

}
