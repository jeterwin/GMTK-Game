using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "LevelsDatabase", menuName = "Scriptable Objects/LevelsDatabase")]
public class LevelsDatabase : ScriptableObject
{
    [SerializeField] List<LevelData> levels;

    [Serializable]
    private class LevelData
    {
        public string name;
        public int timeLimit;
        public int kittens;
    }
}
