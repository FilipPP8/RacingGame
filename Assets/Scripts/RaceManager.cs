using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [SerializeField] private Checkpoint[] _checkpoints;
    public Checkpoint[] Checkpoints => _checkpoints;

    [SerializeField] private int _totalLaps;
    private int _currentLap = 1;

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

        CarCollisionController.OnLapCompleted += UpdateCurrentLap;
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
        
    }

    private void UpdateCurrentLap(bool lapCompleted)
    {
        _currentLap++;
        UIManager.Instance.UpdateLapCount(_currentLap, _totalLaps);
    }
}
