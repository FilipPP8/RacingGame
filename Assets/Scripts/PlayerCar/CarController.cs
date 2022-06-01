using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    public Rigidbody RB => _rb;

    [SerializeField] private bool _isPlayer;

    [Header("Moving and turning")]
    [SerializeField] private float _acceleration; 
    [SerializeField] private float _reverseAcceleration;
    [SerializeField] private float _maxSpeed;
    public float MaxSpeed => _maxSpeed;
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
    private int _currentLap = 1;
    private float _lapTime = 0;
    private float _bestLapTime = 0;

 
    void Start()
    {
        _rb.transform.parent = null;
        _dragOnGround = _rb.drag;
        UIManager.Instance._lapCount.text = _currentLap + "/" + RaceManager.Instance._totalLaps;

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

        _leftFrontWheel.localRotation = Quaternion.Euler(_leftFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelRotation) - 180, _leftFrontWheel.localRotation.eulerAngles.z);
       
        _rightFrontWheel.localRotation = Quaternion.Euler(_rightFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelRotation), _rightFrontWheel.localRotation.eulerAngles.z);



        if(_isGrounded && (Mathf.Abs(_turnInput) > .5f || (_rb.velocity.magnitude < _maxSpeed * 0.5f && _rb.velocity.magnitude !=0)))
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

        _engineSound.pitch = 1f + ((_rb.velocity.magnitude / _maxSpeed)*1.3f);

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

        if(_rb.velocity.magnitude > _maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed; 
        }

        if (_isGrounded && Input.GetAxis("Vertical") != 0)
        {
            _turnVector = new Vector3(0f, _turnInput * _turnForce * Time.deltaTime * Mathf.Sign(_speedInput) * (_rb.velocity.magnitude / _maxSpeed), 0f);

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
}
