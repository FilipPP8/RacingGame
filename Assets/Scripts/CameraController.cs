using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CarController _car;
    private Vector3 _offsetDirection;

    [SerializeField] private float _minDistance, _maxDistance;
    private float _activeDistance;
    void Awake()
    {
        _offsetDirection = transform.position - _car.transform.position;

        _activeDistance = _minDistance;

        _offsetDirection.Normalize();
    
    }

    void Update()
    {
        _activeDistance = _minDistance + ((_maxDistance - _minDistance) * (_car.RB.velocity.magnitude / _car.MaxSpeed));
        transform.position = _car.transform.position + (_offsetDirection * _activeDistance);      
    }
}
