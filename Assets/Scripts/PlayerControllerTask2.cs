using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PlayerControllerTask2 : MonoBehaviour {

	public float movespeed;
	public float maxspeed;
	public float jumpforce;
	public float maxHealth;
	private float health;
    public Slider HPSlider;
	Animator anim;

    int FloorLayer;

	Rigidbody2D playerRB;

	public bool feetContact;

	void Awake() {
		playerRB = gameObject.GetComponent<Rigidbody2D> ();
        anim = GetComponent<Animator>();
        health = maxHealth;
	}

	void Update () {
		float MoveHor = Input.GetAxisRaw ("Horizontal");
		Vector2 movement = new Vector2 (MoveHor * movespeed, 0);
		movement = movement * Time.deltaTime;

		playerRB.AddForce(movement);
		if (playerRB.velocity.x > maxspeed) {
            anim.SetBool("Moving", true);
            playerRB.velocity = new Vector2 (maxspeed, playerRB.velocity.y);
		}
		if (playerRB.velocity.x < -maxspeed) {
            anim.SetBool("Moving", true);
            playerRB.velocity = new Vector2 (-maxspeed, playerRB.velocity.y);
		}
		if (Input.GetKeyDown(KeyCode.Space) && canJump()) {
            playerRB.velocity = new Vector2 (playerRB.velocity.x, 0);
			playerRB.AddForce ( new Vector2(0, jumpforce));
		}
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        anim.SetFloat("DirX", movement.x);
        anim.SetBool("Moving", false);

    }

	bool canJump() {
		return feetContact;
	}

	public void takeDamage()
	{
		health -= 1;
        HPSlider.value = health / maxHealth;
        if (health <= 0) 
		{ 
			Die(); 
		}
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

    #region Interact_functions
    private void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                hit.transform.GetComponent<Chest>().Interact();
            }
        }
    }
    #endregion
}
