using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;

    [SerializeField] private Rigidbody _rb;
    void Start()
    {
        _rb.transform.parent = null;
    }

    void Update()
    {
        _rb.AddForce(Vector3.back);
        transform.position = _rb.transform.position;
    }
}
