using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;

    //Moving and turning
    private float _speedInput, _turnInput;
    [SerializeField] private float _acceleration; 
    [SerializeField] private float _reverseAcceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _turnForce;
    private Vector3 _turnVector;

    // Air control
    private bool _isGrounded;
    [SerializeField] private Transform _groundRayPoint;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _rayLength;
    private float _dragOnGround;
    [SerializeField] private float _gravityModifier;
    
    void Start()
    {
        _rb.transform.parent = null;
        _dragOnGround = _rb.drag;
    }

    void Update()
    {
        _speedInput = 0f;

        if(Input.GetAxis("Vertical") > 0 )
        {
            _speedInput = Input.GetAxis("Vertical") * _acceleration;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            _speedInput = Input.GetAxis("Vertical") * _reverseAcceleration;
        }

        _turnInput = Input.GetAxis("Horizontal");
        
        if (_isGrounded && Input.GetAxis("Vertical") != 0)
        {
            _turnVector = new Vector3(0f,_turnInput * _turnForce * Time.deltaTime * Mathf.Sign(_speedInput) * (_rb.velocity.magnitude / _maxSpeed), 0f);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _turnVector);
        }
        
        transform.position = _rb.transform.position;
    }

    private void FixedUpdate() 
    {
        _isGrounded = false;

        RaycastHit hit;

        if(Physics.Raycast(_groundRayPoint.position, -transform.up, out hit, _rayLength, _whatIsGround))
        {
            _isGrounded = true;
        }

        
        if (_isGrounded)
        {
            _rb.drag = _dragOnGround;
            _rb.AddForce(transform.forward * _speedInput);
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
    }
}
