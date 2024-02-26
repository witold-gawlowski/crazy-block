using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BlockColorList",menuName = "ScriptableObjects/BlockColorList",order = 1)]
public class BlockColorList : ScriptableObject
{
    public List<Color> colorList;
}
