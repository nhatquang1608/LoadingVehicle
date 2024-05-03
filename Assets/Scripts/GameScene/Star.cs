using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private GameObject starWin;
    public void Picked()
    {
        starWin.SetActive(true);
        starWin.transform.DOScale(1f, 0.5f);
    }

    public void SetPicked()
    {
        starWin.SetActive(true);
    }
}
