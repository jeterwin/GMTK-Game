using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "LevelsDatabase", menuName = "Scriptable Objects/LevelsDatabase")]
public class LevelsDatabase : ScriptableObject
{
    [SerializeField] public List<LevelData> levels;

    [Serializable]
    public class LevelData
    {
        public string name;
        public int timeLimit;
        public int kittens;
    }
}
