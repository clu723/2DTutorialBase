using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    #region Health_pack_variables
    [SerializeField]
    [Tooltip("the amount the player heals")]
    private int healingAmount;
    #endregion

    #region Health_functions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().Heal(healingAmount);
            Destroy(this.gameObject);
        }
    }

    #endregion
}
