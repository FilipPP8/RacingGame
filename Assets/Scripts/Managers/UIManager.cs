using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text lapTimer;
    public TMP_Text bestLapTime;
    public TMP_Text lapCount;
    public TMP_Text playerPosition;
    public TMP_Text countdown;
    public TMP_Text resultText;
    public GameObject resultsScreen, pauseScreen;
    public bool isPaused;


    private float _disableTimer = 1f;
    private bool _hasRaceStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        RaceManager.OnRaceStarted += RaceStarted;
    }
    void Start()
    {
        
    }

    private void Update()
    {
        if (_hasRaceStarted)
        {
            _disableTimer -= Time.deltaTime;
            if (_disableTimer <= 0)
            {
                countdown.gameObject.SetActive(false);
                _hasRaceStarted = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void RaceStarted(TMP_Text textToDisable)
    {
        _hasRaceStarted = true;

    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.Instance.ExitRace();
        resultsScreen.SetActive(false);
    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
