using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedStar : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            gameManager.PickedStar();
            Destroy(gameObject);
        }
    }
}
