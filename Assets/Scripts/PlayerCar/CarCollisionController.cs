using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionController : MonoBehaviour
{
    [SerializeField] CarController _car;
    [SerializeField] AudioSource _hitSound;
    private int _groundLayerNumber = 7;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != _groundLayerNumber)
        {
            _hitSound.Stop();
            _hitSound.pitch = Random.Range(0.8f, 1.2f);
            _hitSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            
            _car.CheckpointHit(other.GetComponent<Checkpoint>().checkpointNumber);

        }
    }

}
