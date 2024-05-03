using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool isGoingUp = true;

    private void Update()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if(isGoingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, topPoint.position, speed * Time.deltaTime);
            if(transform.position == topPoint.position)
            {
                isGoingUp = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, bottomPoint.position, speed * Time.deltaTime);
            if(transform.position == bottomPoint.position)
            {
                isGoingUp = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.tag == "Player")
        {
            if(!isGoingUp && collider.transform.position.y < transform.position.y) 
            {
                isGoingUp = true;
            }
            else if(!collider.GetComponent<RobotController>().holdingNail)
            {
                collider.transform.SetParent(transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.tag == "Player" && !collider.GetComponent<RobotController>().holdingNail)
        {
            collider.transform.SetParent(null);
        }
    }
}
