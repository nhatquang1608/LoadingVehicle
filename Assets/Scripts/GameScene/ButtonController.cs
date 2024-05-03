using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private enum ControlType
    {
        Left,
        Right,
    }

    [SerializeField] private ControlType controlType;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch(controlType)
        {
            case ControlType.Left:
                RobotController.moveDirection = new Vector2(-1, RobotController.moveDirection.y);
                break;
            case ControlType.Right:
                RobotController.moveDirection = new Vector2(1, RobotController.moveDirection.y);
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch(controlType)
        {
            case ControlType.Left:
                RobotController.moveDirection = new Vector2(0, RobotController.moveDirection.y);
                break;
            case ControlType.Right:
                RobotController.moveDirection = new Vector2(0, RobotController.moveDirection.y);
                break;
        }
    }
}
