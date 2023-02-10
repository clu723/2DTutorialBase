using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    #region movement_variables
    // public so we can access outside of here too
    public float moveSpeed;
    float x_input;
    float y_input;
    #endregion

    #region physics_components
    Rigidbody2D PlayerRB; // controls the forces we add to our player
    #endregion

    #region attack_variables
    public float damage;
    // how long player has to wait to attack again
    public float attackSpeed = 3;
    float attackTimer;
    // vars for making player only do dmg when sword swings
    public float hitBoxTiming;
    public float endAnimationTiming;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;
    public Slider HPSlider;
    #endregion

    #region animation_components
    Animator anim;
    #endregion

    #region unity_functions
    // basically a start function
    private void Awake()
    {   
        // accesses the component type
        PlayerRB = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        anim = GetComponent<Animator>();
        currHealth = maxHealth;
        HPSlider.value = currHealth / maxHealth;
    }

    // Updates every frame
    private void Update()
    {   
        // if player is currently attacking, don't run any other inputs
        if (isAttacking)
        {
            return;
        }

        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
        {
            Attack();
        } else
        {
            // time.deltaTime keeps track of time in between frames
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    #endregion

    #region movement_functions

    // move the player on wasd inputs
    private void Move()
    {
        anim.SetBool("Moving", true);

        if (x_input > 0)
        {
            // moves player right
            PlayerRB.velocity = Vector2.right * moveSpeed;
            currDirection = Vector2.right;
        } else if (x_input < 0)
        {
            // moves player left
            PlayerRB.velocity = Vector2.left * moveSpeed;
            currDirection = Vector2.left;
        }
            else if (y_input > 0)
        {
            // moves player up
            PlayerRB.velocity = Vector2.up * moveSpeed;
            currDirection = Vector2.up;
        }
            // moves player down
            else if (y_input < 0)
        {
            PlayerRB.velocity = Vector2.down * moveSpeed;
            currDirection = Vector2.down;

            // reset
        } else
        {
            PlayerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }

        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region attack_functions
    private void Attack()
    {
        Debug.Log("attacking now");
        Debug.Log(currDirection);
        attackTimer = attackSpeed;
        // handles animation and hitbox
        StartCoroutine(attackRoutine());
    }
    // runs same time as update; similar to generators
    IEnumerator attackRoutine()
    {
        isAttacking = true;
        PlayerRB.velocity = Vector2.zero;

        anim.SetTrigger("Attacktrig");

        // start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerAttack");

        // brief pause before casting hitbox
        yield return new WaitForSeconds(hitBoxTiming);
        Debug.Log("Casting hitbox now");
        // Projects box in front of player, returns anything that box hits
        // 0f = 0 in float
        // Vector2.one = 1x1 box
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);
        
        // for each thing we hit
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.transform.name);
            // if it's an enemy
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("tons of damage");
                // gets enemy and calls takedamage func
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        // wait for animation to end before we allow player to move again
        yield return new WaitForSeconds(endAnimationTiming);
        isAttacking = false;

        yield return null;
    }
    #endregion

    #region Health_functions
    // take damage based on value param passed in by caller
    public void TakeDamage(float damage)
    {
        // play sound effect
        FindObjectOfType<AudioManager>().Play("PlayerHurt");

        currHealth -= damage;
        Debug.Log("Player Health is now: " + currHealth.ToString());

        // change UI
        HPSlider.value = currHealth / maxHealth;

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
        Debug.Log("Health is now: " + currHealth.ToString());
        HPSlider.value = currHealth / maxHealth;
    }

    private void Die()
    {
        // play sound effect
        FindObjectOfType<AudioManager>().Play("PlayerDeath");

        // destroy this object
        Destroy(this.gameObject);

        // trigger anything to end the game, trigger gamemanager and lose the game
        GameObject gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();
    }


    #endregion

    #region Interact_functions
    private void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                hit.transform.GetComponent<Chest>().Interact();
            }
        }
    }

    #endregion
}