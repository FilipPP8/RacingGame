using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text _lapTimer;
    public TMP_Text _bestLapTime;
    public TMP_Text _lapCount;
    public TMP_Text _playerPosition;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        
    }

}
