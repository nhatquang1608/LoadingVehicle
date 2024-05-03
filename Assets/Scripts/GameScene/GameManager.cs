using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event Action OnJump;
    public static event Action OnGrab;

    public bool isGameOver;
    public bool waiting;
    [SerializeField] private float time;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private int pickedStarsCount;
    [SerializeField] private int completedStarsCount;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private Button nextButton;
    [SerializeField] private Star[] listGameStar;
    [SerializeField] private Star[] listGameOverStar;
    [SerializeField] private GameObject[] listMaps;
    [SerializeField] private RobotController robotController;

    private void Awake()
    {
        levelText.text = "Level " + (SaveLoadData.Instance.level + 1).ToString();
        listMaps[SaveLoadData.Instance.level].SetActive(true);
    }

    private void Start()
    {
        time = 60;
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while(!isGameOver)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);

            if (time <= 0)
            {
                time = 0;
                GameOver(false);
            }

            yield return new WaitForSeconds(1);

            time --;
        }
    }

    public void GameOver(bool completed)
    {
        isGameOver = true;
        if(completed)
        {
            StartCoroutine(SetCompletedPanel());
        }
        else
        {
            gameOverPanel.SetActive(true);
            failedPanel.SetActive(true);
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].pickedStars = 0;
            SaveLoadData.Instance.SaveData();
        }
    }

    private IEnumerator SetCompletedPanel()
    {
        waiting = true;
        SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].pickedStars = pickedStarsCount;
        if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count - 1)
        {
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level+1].isLock = false;
        }
        SaveLoadData.Instance.SaveData();

        yield return new WaitForSeconds(2);

        gameOverPanel.SetActive(true);
        completedPanel.SetActive(true);
        while(completedStarsCount < pickedStarsCount)
        {
            listGameOverStar[completedStarsCount].Picked();
            completedStarsCount++;
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        waiting = false;
    }

    public void PickedStar()
    {
        listGameStar[pickedStarsCount].Picked();
        pickedStarsCount++;
    }

    public void Jump()
    {
        OnJump?.Invoke();
    }

    public void Grab()
    {
        OnGrab?.Invoke();
    }

    public void RestartGame()
    {
        if(!waiting)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void NextGame()
    {
        if(!waiting)
        {
            if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count - 1)
            {
                SaveLoadData.Instance.level++;
            }
            else
            {
                SaveLoadData.Instance.level = 0;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Back()
    {
        if(!waiting)
        {
            SceneManager.LoadScene("SelectScene");
        }
    }
}
