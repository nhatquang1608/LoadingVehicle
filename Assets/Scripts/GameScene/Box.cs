using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool canBeGrabbed;
    public bool canBeThrown;
    [SerializeField] private GameObject canBeGrabbedSign;
    [SerializeField] private RobotController robotController;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = true;
            canBeGrabbedSign.SetActive(true);
            robotController.box = transform;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = true;
            canBeGrabbedSign.SetActive(true);
            robotController.box = transform;
        }
        else if(collider.transform.tag == "Truck")
        {
            canBeThrown = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = false;
            canBeGrabbedSign.SetActive(false);
            robotController.box = null;
        }
        else if(collider.transform.tag == "Truck")
        {
            canBeThrown = true;
        }
    }
}
