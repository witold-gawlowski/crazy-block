using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUIScript : MonoBehaviour
{
    [SerializeField] private GameObject clockPrefab;

    public GameObject SpawnClock()
    {
        return Instantiate(clockPrefab);
    }
}
