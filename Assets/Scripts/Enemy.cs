using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Movement_variables
    public float moveSpeed;
    #endregion

    #region Physics_components
    Rigidbody2D EnemyRB;
    #endregion

    #region Targeting_variables
    public Transform player;
    #endregion

    #region Attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObject;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region Unity_functions
    // runs once on creation
    private void Awake()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
    }

    // runs every frame
    private void Update()
    {
        // check to see if it knows where player is
        if (!player)
        {
            return;
        }

        Move();
    }
    #endregion

    #region Movement_functions
    private void Move()
    {
        // Calculate movement vector player position - enemy position = direction of player relative to enemy
        Vector2 direction = player.position - transform.position;

        // normalize b/c this would be massive if direction is massive
        EnemyRB.velocity = direction.normalized * moveSpeed;
    }
    #endregion

    #region Attack_functions
    // Raycasts box for player, causes damage, spawns explosion prefab
    private void Explode()
    {
        // play sound effect
        FindObjectOfType<AudioManager>().Play("Explosion");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                // Cause damage
                Debug.Log("Hit Player with explosion");

                // spawn explosion prefab
                Instantiate(explosionObject, transform.position, transform.rotation);
                hit.transform.GetComponent<PlayerController>().TakeDamage(explosionDamage);
                Destroy(this.gameObject);
            }
        }
    }

    // If collider detects an object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Explode();
        }
    }
    #endregion

    #region Health_functions
    // take damage based on value param passed in by caller
    public void TakeDamage(float damage)
    {
        // play sound effect
        FindObjectOfType<AudioManager>().Play("BatHurt");

        currHealth -= damage;
        Debug.Log("Health is now: " + currHealth.ToString());

        // change UI

        // Check if dead

        if (currHealth <= 0)
        {
            Die();
        }
    }

    // Heals player on value passed in by caller
    public void Heal(float val)
    {
        // increment health by value
        currHealth += val;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Enemy Health is now: " + currHealth.ToString());
    }

    private void Die()
    {
        // destroy this object
        Destroy(this.gameObject);
    }


    #endregion

}
