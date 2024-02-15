using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MapGeneratorInterface
{
    public void Init();
    public void Next();
    public GameObject GetCurrent();
    public int GetCurrentReward();
}
