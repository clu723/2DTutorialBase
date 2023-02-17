using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanScript : MonoBehaviour
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

    private void Awake()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
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

    #region Movement_functions
    private void Move()
    {
        // Calculate movement vector player position - enemy position = direction of player relative to enemy
        Vector2 direction = player.position - transform.position;

        // normalize b/c this would be massive if direction is massive
        EnemyRB.velocity = direction.normalized * moveSpeed;
    }
    #endregion

    private void Attack()
    {
        // play sound effect
        FindObjectOfType<AudioManager>().Play("Explosion");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerControllerTask2>().takeDamage();
            }
        }
    }

    // If collider detects an object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Attack();
        }
    }
}
