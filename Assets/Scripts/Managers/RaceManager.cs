using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [SerializeField] private Checkpoint[] _checkpoints;
    public Checkpoint[] Checkpoints => _checkpoints;

    [SerializeField] private int _totalLaps;
    private int _currentLap = 1;

    private bool _timerStart;
    private float _lapTime;
    private float _bestLap;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CarCollisionController.OnLapCompleted += UpdateRaceInfo;
        CarCollisionController.OnRaceStarted += RaceStarted;

        _timerStart = false;
        _lapTime = 0;
        _bestLap = 0;
    }
    void Start()
    {
        for(int i = 0; i < _checkpoints.Length; i++)
        {
            _checkpoints[i].checkpointNumber = i;
        }
        UIManager.Instance.UpdateLapCount(_currentLap, _totalLaps);
    }

    void Update()
    {
        if(_timerStart == true)
        {
            _lapTime += Time.deltaTime;
            TimeSpan lapTimeSpan = TimeSpan.FromSeconds(_lapTime);
            UIManager.Instance.UpdateLapTime(lapTimeSpan);
        }
    }

    private void UpdateRaceInfo(bool lapCompleted)
    {
        _currentLap++;
        UIManager.Instance.UpdateLapCount(_currentLap, _totalLaps);

        if (_lapTime < _bestLap || _bestLap == 0)
        {
            _bestLap = _lapTime;
            TimeSpan bestLapTimeSpan = TimeSpan.FromSeconds(_bestLap);
            UIManager.Instance.UpdateBestLap(bestLapTimeSpan);
        }

        _lapTime = 0;
     
    }

    private void RaceStarted(bool raceStarted)
    {
        _timerStart = true;
    }
}
