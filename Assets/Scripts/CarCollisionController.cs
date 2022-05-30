using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionController : MonoBehaviour
{
    public static event System.Action<bool> OnCarCollided;

    private int _groundLayerNumber = 7;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != _groundLayerNumber)
        {
            OnCarCollided?.Invoke(true);
        }
    }
}
