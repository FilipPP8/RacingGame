using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public Checkpoint[] _checkpoints;

    public int _totalLaps;

    [SerializeField] private CarController _playerCar;
    [SerializeField] private List<CarController> _aICars = new List<CarController>();
    private int _playerPosition;
    private float _positionCheckTimer;

    private float _playerDefaultSpeed = 30f;
    private float _aiDefaultSpeed = 30f;
    private float _rubberBandSpeedMod = 3.5f;
    private float _rubberBandAcceleration = 0.5f;

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

    private void Update()
    {
        _positionCheckTimer -= Time.deltaTime;

        if(_positionCheckTimer <= 0)
        {
            CheckPlayerPosition();
            UIManager.Instance._playerPosition.text = _playerPosition + "/" + (_aICars.Count+1);
            _positionCheckTimer = 0.2f;
        }

        if(_playerPosition == 1)
        {
            foreach(CarController car in _aICars)
            {
                car.maxSpeed = Mathf.MoveTowards(car.maxSpeed, _aiDefaultSpeed + _rubberBandSpeedMod, _rubberBandAcceleration * Time.deltaTime);
            }

            _playerCar.maxSpeed = Mathf.MoveTowards(_playerCar.maxSpeed, _playerDefaultSpeed - _rubberBandSpeedMod, _rubberBandAcceleration * Time.deltaTime);
        }
        else
        {
            var playerSpeedMod = _rubberBandSpeedMod * ((float)_playerPosition / (float)(_aICars.Count + 1));

            foreach (CarController car in _aICars)
            {
                car.maxSpeed = Mathf.MoveTowards(car.maxSpeed, _aiDefaultSpeed - playerSpeedMod, _rubberBandAcceleration * Time.deltaTime);
            }

            _playerCar.maxSpeed = Mathf.MoveTowards(_playerCar.maxSpeed, _playerDefaultSpeed + playerSpeedMod, _rubberBandAcceleration * Time.deltaTime);

        }

    }

    private void CheckPlayerPosition()
    {
        _playerPosition = 1;

        foreach (CarController car in _aICars)
        {
            if (car.CurrentLap > _playerCar.CurrentLap)
            {
                _playerPosition++;
            }
            else if (car.CurrentLap == _playerCar.CurrentLap)
            {
                if (car.NextCheckpoint > _playerCar.NextCheckpoint)
                {
                    _playerPosition++;
                }
                else if (car.NextCheckpoint == _playerCar.NextCheckpoint)
                {
                    var nextCpPos = _checkpoints[car.NextCheckpoint].transform.position;

                    if (Vector3.Distance(car.transform.position, nextCpPos) < Vector3.Distance(_playerCar.transform.position, nextCpPos))
                    {
                        _playerPosition++;
                    }
                }
            }

        }
    }



}
