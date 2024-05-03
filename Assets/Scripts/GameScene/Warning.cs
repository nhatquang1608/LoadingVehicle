using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Warning : MonoBehaviour
{
    public bool pressed;
    [SerializeField] private int objectsPressed;
    [SerializeField] private Bars bars;
    [SerializeField] private Transform warningButton;
    [SerializeField] private Transform warningButtonDefaultPoint;
    [SerializeField] private Transform warningButtonChangedPoint;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player" || collider.transform.tag == "Box")
        {
            warningButton.DOMove(warningButtonChangedPoint.position, 0.1f);
            pressed = true;
            objectsPressed++;
            bars.ShowHide();
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player" || collider.transform.tag == "Box")
        {
            // warningButton.DOMove(warningButtonChangedPoint.position, 0.1f);
            pressed = true;
            // bars.ShowHide();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.transform.tag == "Player" || collider.transform.tag == "Box")
        {
            objectsPressed--;
            if(objectsPressed <= 0)
            {
                warningButton.DOMove(warningButtonDefaultPoint.position, 0.1f);
                pressed = false;
                bars.ShowHide();
            }
        }
    }
}
