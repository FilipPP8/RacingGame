using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionController : MonoBehaviour
{
    public static event System.Action<bool> OnCarCollided;
    private int _groundLayerNumber = 7;

    private int _nextCheckpoint = 0;
    private int _completedLaps = 0;

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
                Debug.Log("Hit checkpoint: " + cpNumber);

                _nextCheckpoint++;
            }

            if (_nextCheckpoint == RaceManager.Instance.Checkpoints.Length)
            {
                _nextCheckpoint = 0;
                _completedLaps++;
                Debug.Log("You've completed a lap!");
            }


        }
    }

}
