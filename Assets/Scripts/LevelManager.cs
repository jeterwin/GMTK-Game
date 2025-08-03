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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            Time.timeScale = 0;
            winScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
