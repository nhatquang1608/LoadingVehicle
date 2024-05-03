using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bars : MonoBehaviour
{
    [SerializeField] private Transform defaultPoint;
    [SerializeField] private Transform changedPoint;
    [SerializeField] private Warning[] listWarnings;

    public void ShowHide()
    {
        foreach(Warning warning in listWarnings)
        {
            if(warning.pressed)
            {
                transform.DOMove(changedPoint.position, 0.1f);
                return;
            }
        }
        transform.DOMove(defaultPoint.position, 0.1f);
    }
}
