using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 1)]
public class GameData : ScriptableObject
{
    public int resourceValue;
    public string playerID;
    public bool isZoneCaptured;
    public float timer;
    
}
