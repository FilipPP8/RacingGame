using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public Checkpoint[] _checkpoints;

    public int _totalLaps;

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

    }
    void Start()
    {
        for(int i = 0; i < _checkpoints.Length; i++)
        {
            _checkpoints[i].checkpointNumber = i;
        }
    }



}
