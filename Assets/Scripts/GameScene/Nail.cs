using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : MonoBehaviour
{
    public bool canBeGrabbed;
    [SerializeField] private GameObject canBeGrabbedSign;
    [SerializeField] private RobotController robotController;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = true;
            canBeGrabbedSign.SetActive(true);
            robotController.nail = transform;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = true;
            canBeGrabbedSign.SetActive(true);
            robotController.nail = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player")
        {
            canBeGrabbed = false;
            canBeGrabbedSign.SetActive(false);
            robotController.nail = null;
        }
    }
}
