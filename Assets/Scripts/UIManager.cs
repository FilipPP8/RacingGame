using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] TMP_Text _lapTimer;
    [SerializeField] TMP_Text _bestLapTime;
    [SerializeField] TMP_Text _lapCount;
    

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
    public void UpdateLapCount(int currentLap, int totalLaps)
    {
        _lapCount.text = currentLap + "/" + totalLaps;
    }
}
