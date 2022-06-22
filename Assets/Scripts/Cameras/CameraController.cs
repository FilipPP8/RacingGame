using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController _targetCar;
    private Vector3 _offsetDirection;

    [SerializeField] private float _minDistance, _maxDistance;
    private float _activeDistance;

    [SerializeField] private Transform _startTargetOffset;
    void Awake()
    {
        _offsetDirection = transform.position - _startTargetOffset.position;

        _activeDistance = _minDistance;

        _offsetDirection.Normalize();
    
    }

    void Update()
    {
        _activeDistance = _minDistance + ((_maxDistance - _minDistance) * (_targetCar._rb.velocity.magnitude / _targetCar.maxSpeed));
        transform.position = _targetCar.transform.position + (_offsetDirection * _activeDistance);      
    }
}
