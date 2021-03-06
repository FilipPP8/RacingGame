using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public static event Action<TMP_Text> OnRaceStarted;

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

    public bool isStarting;
    private float _timeBetweenCounts = 1f;
    private float _startCounter;
    private int _countdownCurrent = 3;

    [SerializeField] private Transform[] _startPositions;
    public int playerStartPosition;
    public int aiNumberToSpawn;
    [SerializeField] private List<CarController> _carsToSpawn = new List<CarController>();

    public bool _raceCompleted;

    public string raceCompleteScene;

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
        _totalLaps = RaceInfoManager.Instance.lapsNumber;
        aiNumberToSpawn = RaceInfoManager.Instance.aiNumber;

        for(int i = 0; i < _checkpoints.Length; i++)
        {
            _checkpoints[i].checkpointNumber = i;
        }

        isStarting = true;
        _startCounter = _timeBetweenCounts;
        UIManager.Instance.countdown.text = _countdownCurrent + "!";

        playerStartPosition = UnityEngine.Random.Range(0,aiNumberToSpawn+1);

        _playerCar = Instantiate(RaceInfoManager.Instance.racerToUse, _startPositions[playerStartPosition].position, _startPositions[playerStartPosition].rotation);
        _playerCar.isPlayer = true;
        _playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitch.Instance.SetTarget(_playerCar);

        for(int i = 0; i < aiNumberToSpawn+1; i++)
        {
            if(i != playerStartPosition)
            {
                int selectedCar = UnityEngine.Random.Range(0, _carsToSpawn.Count);
                _aICars.Add(Instantiate(_carsToSpawn[selectedCar], _startPositions[i].position, _startPositions[i].rotation));

                if (_carsToSpawn.Count > aiNumberToSpawn - i)
                {
                    _carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }
        UIManager.Instance.playerPosition.text = (playerStartPosition+1) + "/" + (_aICars.Count + 1);

    }

    private void Update()
    {

        if (isStarting)
        {

            _startCounter -= Time.deltaTime;
            if(_startCounter <= 0)
            {
                _countdownCurrent--;
                _startCounter = _timeBetweenCounts;
                UIManager.Instance.countdown.text = _countdownCurrent + "!";

                if (_countdownCurrent == 0)
                {
                    isStarting = false;
                    UIManager.Instance.countdown.text = "GO!";
                    OnRaceStarted?.Invoke(UIManager.Instance.countdown);
                }
            }
        }
        else
        {


            _positionCheckTimer -= Time.deltaTime;

            if (_positionCheckTimer <= 0)
            {
                CheckPlayerPosition();
                UIManager.Instance.playerPosition.text = _playerPosition + "/" + (_aICars.Count + 1);
                _positionCheckTimer = 0.2f;
            }

            if (_playerPosition == 1)
            {
                foreach (CarController car in _aICars)
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

    public void FinishRace()
    {
        _raceCompleted = true;

        switch(_playerPosition)
        {
            case 1:
                
                UIManager.Instance.resultText.text = "You finished 1st!";

                break;

            case 2:
                UIManager.Instance.resultText.text = "You finished 2nd!";
                break;
            case 3:
                UIManager.Instance.resultText.text = "You finished 3rd!";
                break;
            default:
                UIManager.Instance.resultText.text = "You finished " + _playerPosition +"th!";
                break;
        }

        UIManager.Instance.resultsScreen.SetActive(true);
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompleteScene);
    }

}
