using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarCollisionController : MonoBehaviour
{
    public static event Action<bool> OnCarCollided;
    public static event Action<bool> OnLapCompleted;
    public static event Action<bool> OnRaceStarted;

    private int _groundLayerNumber = 7;

    private int _nextCheckpoint = 0;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != _groundLayerNumber)
        {
            OnCarCollided?.Invoke(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkpoint")
        {
            int cpNumber = other.GetComponent<Checkpoint>().checkpointNumber;
            if(cpNumber == _nextCheckpoint)
            {
                _nextCheckpoint++;
            }

            if (_nextCheckpoint == RaceManager.Instance.Checkpoints.Length)
            {
                _nextCheckpoint = 0;
                LapCompleted();
            }
        }
        if(other.tag == "Start")
        {
            Destroy(other);
            OnRaceStarted?.Invoke(true);
        }
    }

    private void LapCompleted()
    {
            OnLapCompleted?.Invoke(true);
    }

}
