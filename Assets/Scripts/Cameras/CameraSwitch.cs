using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance;

    [SerializeField] private GameObject[] _cameras;

    public CameraController isometricCam;
    public CinemachineVirtualCamera followCam;

    private void Awake()
    {
        Instance = this;
    }
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

    public void SetTarget(CarController playerCar)
    {
        isometricCam._targetCar = playerCar;
        followCam.m_Follow = playerCar.transform;
        followCam.m_LookAt = playerCar.transform;
    }
}
