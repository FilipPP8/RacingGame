using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private bool _isPlayer;

    public Rigidbody RB => _rb;
    [Header("Moving and turning")]
    [SerializeField] private float _acceleration; 
    [SerializeField] private float _reverseAcceleration;
    public float maxSpeed;
    [SerializeField] private float _turnForce;
    private Vector3 _turnVector;
    private float _speedInput, _turnInput;

    [Header("Air and ground control")]
    [SerializeField] private Transform _groundRayPoint;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _rayLength;
    [SerializeField] private float _gravityModifier;
    private bool _isGrounded;
    private float _dragOnGround;
    
    [Header("Car angle")]
    [SerializeField] private Transform _surfaceAngleRayPoint;

    [Header("Wheels rotation")]
    [SerializeField] private Transform _leftFrontWheel;
    [SerializeField] private Transform _rightFrontWheel;
    private float _maxWheelRotation = 25f;

    [Header("Trails")]
    [SerializeField] private ParticleSystem[] _dustTrails;
    [SerializeField] private float _maxEmission;
    private float _emissionFadeSpeed = 50f;
    private float _emissionRate;

    [Header("SFX")]
    [SerializeField] private AudioSource _engineSound;
    [SerializeField] private AudioSource _skidSound;
    private float _skidSoundFade = 2;

    
    private int _nextCheckpoint = 0;
    public int NextCheckpoint => _nextCheckpoint;
    private int _currentLap = 1;
    public int CurrentLap => _currentLap;
    private float _lapTime = 0;
    private float _bestLapTime = 0;

    [Header("AI")]
    [SerializeField] private int _currentTarget;
    private Vector3 _targetPosition;
    private float _aiAccelerateSpeed = 1f;
    private float _aiTurnSpeed = 0.8f;
    private float _aiMaxTurn = 15f;
    private float _aiReachPointRange = 5f;
    private float _aiPointVariance = 3f;
    private float _aiSpeedInput;
    private float _aiSpeedModifier;
 
    void Start()
    {
        _rb.transform.parent = null;
        _dragOnGround = _rb.drag;
        UIManager.Instance._lapCount.text = _currentLap + "/" + RaceManager.Instance._totalLaps;

        if(!_isPlayer)
        {
            _targetPosition = RaceManager.Instance._checkpoints[_currentTarget].transform.position;
            RandomizeAITarget();

            _aiSpeedModifier = Random.Range(0.8f, 1.1f);
        }

    }

    void Update()
    {
        _lapTime += Time.deltaTime;
        if (_isPlayer)
        {
            var lapTS = System.TimeSpan.FromSeconds(_lapTime);
            UIManager.Instance._lapTimer.text = string.Format("{0:00}m{1:00}.{2:00}s", lapTS.Minutes, lapTS.Seconds, lapTS.Milliseconds);
            
            _speedInput = 0f;

            if (Input.GetAxis("Vertical") > 0)
            {
                _speedInput = Input.GetAxis("Vertical") * _acceleration;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                _speedInput = Input.GetAxis("Vertical") * _reverseAcceleration;
            }

            _turnInput = Input.GetAxis("Horizontal");
        }
        else
        {
            _targetPosition.y = transform.position.y;
            if(Vector3.Distance(transform.position,_targetPosition) < _aiReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 _targetDirection = _targetPosition - transform.position;
            float angle = Vector3.Angle(_targetDirection, transform.forward);

            Vector3 localPosition = transform.InverseTransformPoint(_targetPosition);
            if(localPosition.x < 0f)
            {
                angle = -angle;
            }

            _turnInput = Mathf.Clamp(angle/_aiMaxTurn, -1f, 1f);

            if(Mathf.Abs(angle) < _aiMaxTurn)
            {
                _aiSpeedInput = Mathf.MoveTowards(_aiSpeedInput, 1f, _aiAccelerateSpeed);
            }
            else
            {
                _aiSpeedInput = Mathf.MoveTowards(_aiSpeedInput, _aiTurnSpeed, _aiAccelerateSpeed);
            }

            _aiSpeedInput = 1f;
            _speedInput = _aiSpeedInput * _acceleration * _aiSpeedModifier;
        }

        _leftFrontWheel.localRotation = Quaternion.Euler(_leftFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelRotation) - 180, _leftFrontWheel.localRotation.eulerAngles.z);
       
        _rightFrontWheel.localRotation = Quaternion.Euler(_rightFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelRotation), _rightFrontWheel.localRotation.eulerAngles.z);



        if(_isGrounded && (Mathf.Abs(_turnInput) > .5f || (_rb.velocity.magnitude < maxSpeed * 0.5f && _rb.velocity.magnitude !=0)))
        {
            _emissionRate = _maxEmission;
        }

        if(_rb.velocity.magnitude <= 0.5f)
        {
            _emissionRate = 0;
        }

        _emissionRate = Mathf.MoveTowards(_emissionRate, 0f, _emissionFadeSpeed * Time.deltaTime);

        foreach (ParticleSystem dustTrail in _dustTrails)
        {
            var emissionModule = dustTrail.emission;

            emissionModule.rateOverTime = _emissionRate;
        }

        _engineSound.pitch = 1f + ((_rb.velocity.magnitude / maxSpeed)*1.3f);

        if (_isGrounded && Mathf.Abs(_turnInput) > 0.5f)
        {
            _skidSound.volume = 1f;
        }
        else
        {
            _skidSound.volume = Mathf.MoveTowards(_skidSound.volume, 0f, _skidSoundFade * Time.deltaTime);
        }
    }

    private void FixedUpdate() 
    {
        _isGrounded = false;

        RaycastHit hit;

        Vector3 normalTarget = Vector3.zero;

        if(Physics.Raycast(_groundRayPoint.position, -transform.up, out hit, _rayLength, _whatIsGround))
        {
            _isGrounded = true;

            normalTarget = hit.normal;
        }

        if(Physics.Raycast(_surfaceAngleRayPoint.position, -transform.up, out hit, _rayLength, _whatIsGround))
        {
            _isGrounded = true;

            normalTarget = (normalTarget + hit.normal)/ 2;
        }

        if (_isGrounded)
        {
            _rb.drag = _dragOnGround;
            _rb.AddForce(transform.forward * _speedInput);

            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }
        else
        {
            _rb.drag = 0.2f;
            _rb.AddForce(-Vector3.up * _gravityModifier);
        }

        if(_rb.velocity.magnitude > maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * maxSpeed; 
        }

        if (_isGrounded && _speedInput != 0)
        {
            _turnVector = new Vector3(0f, _turnInput * _turnForce * Time.deltaTime * Mathf.Sign(_speedInput) * (_rb.velocity.magnitude / maxSpeed), 0f);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _turnVector);
        }



        if(Input.GetKey(KeyCode.R))
        {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }

       transform.position = _rb.transform.position;

    }

    public void CheckpointHit(int cpNumber)
    {
        if (cpNumber == _nextCheckpoint)
        {
            _nextCheckpoint++;

            if (_nextCheckpoint == RaceManager.Instance._checkpoints.Length)
            {
                _nextCheckpoint = 0;
                LapCompleted();
            }
        }

        if(!_isPlayer)
        {
            if(cpNumber == _currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void LapCompleted()
    {
        _currentLap++;

        if (_lapTime < _bestLapTime || _bestLapTime == 0)
        {
            _bestLapTime = _lapTime;
        }

        _lapTime = 0f;

        if (_isPlayer)
        {
                var bestLapTS = System.TimeSpan.FromSeconds(_bestLapTime);
                UIManager.Instance._bestLapTime.text = string.Format("{0:00}m{1:00}.{2:000}s", bestLapTS.Minutes, bestLapTS.Seconds, bestLapTS.Milliseconds);

                UIManager.Instance._lapCount.text = _currentLap + "/" + RaceManager.Instance._totalLaps;
        }   
    }

    public void RandomizeAITarget()
    {
        _targetPosition += new Vector3(Random.Range(-_aiPointVariance, _aiPointVariance), 0f, Random.Range(-_aiPointVariance, _aiPointVariance));
    }

    public void SetNextAITarget()
    {
        _currentTarget++;
        if (_currentTarget >= RaceManager.Instance._checkpoints.Length)
        {
            _currentTarget = 0;
        }
        _targetPosition = RaceManager.Instance._checkpoints[_currentTarget].transform.position;
        RandomizeAITarget();
    }
}
