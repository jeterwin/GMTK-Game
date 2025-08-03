using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int level;
    [SerializeField] LevelsDatabase levelsDatabase;
    [SerializeField] GameObject winScreen;

    private int kittensTotal;
    public int timeLimit;
    private int kittensFound;

    void Awake()
    {
        Instance = this;
        kittensTotal = levelsDatabase.GetKittens(level);
        timeLimit = levelsDatabase.GetTimeLimit(level);
    }

    public void KittenFound()
    {
        kittensFound++;
        if (kittensFound >= kittensTotal)
        {
            DialogueManager.Instance.dialogueBox.SetActive(false);
            Time.timeScale = 0;
            winScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 🔓 Unlock the next level
            int nextLevelIndex = level + 1;
            if (nextLevelIndex < levelsDatabase.levels.Count)
            {
                SaveSystem.UnlockLevel(nextLevelIndex);
            }

            // 💾 Save progress (including "GameStarted")
            SaveSystem.SetGameStarted();
            SaveSystem.SaveProgress();
        }
    }
}
