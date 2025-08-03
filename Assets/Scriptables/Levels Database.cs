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

    public int GetTimeLimit(int level)
    {
        return levels[level].timeLimit;
    }

    public int GetKittens(int level)
    {
        return levels[level].kittens;
    }
}
