using UnityEngine;

public static class SaveSystem
{
    public static void SetGameStarted() => PlayerPrefs.SetInt("GameStarted", 1);
    public static bool HasGameStarted() => PlayerPrefs.GetInt("GameStarted", 0) == 1;

    public static void UnlockLevel(int index) => PlayerPrefs.SetInt($"LevelUnlocked_{index}", 1);
    public static bool IsLevelUnlocked(int index) => PlayerPrefs.GetInt($"LevelUnlocked_{index}", index == 0 ? 1 : 0) == 1;

    public static void SaveProgress() => PlayerPrefs.Save();
}
