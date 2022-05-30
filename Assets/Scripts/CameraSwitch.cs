using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private GameObject[] _cameras;

    private void Start()
    {
        _cameras[0].SetActive(true);
        _cameras[1].SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCameras();
        }
    }

    void SwitchCameras()
    {
            if(_cameras[0].activeInHierarchy == true)
            {
                _cameras[0].SetActive(false);
                _cameras[1].SetActive(true);
            }
            else
            {
                _cameras[0].SetActive(true);
                _cameras[1].SetActive(false);
            }
    }
}
