using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] TMP_Text _lapTimer;
    [SerializeField] TMP_Text _bestLapTime;
    [SerializeField] TMP_Text _lapCount;
    

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
    }
    void Start()
    {
        
    }
    public void UpdateLapCount(int currentLap, int totalLaps)
    {
        _lapCount.text = currentLap + "/" + totalLaps;
    }

    public void UpdateLapTime(TimeSpan lT)
    {

        _lapTimer.text = string.Format("{0:00}m{1:00}.{2:000}s", lT.Minutes, lT.Seconds, lT.Milliseconds);
    }

    public void UpdateBestLap(TimeSpan bL)
    {
        _bestLapTime.text = string.Format("{0:00}m{1:00}.{2:000}s", bL.Minutes, bL.Seconds, bL.Milliseconds);
    }
}
