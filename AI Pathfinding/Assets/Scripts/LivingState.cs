using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//would be used when the player gets touched by a seeker that he loses the level and dies after some hits by the enemy 
public class LivingState : MonoBehaviour {

    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth; //players starting health 
    }

  //  public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)

    public virtual void TakeDamage(float damage) //when the player has taken hits and reached health 0, he dies  
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void Die()  // disposing of the object when dead
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }


}
