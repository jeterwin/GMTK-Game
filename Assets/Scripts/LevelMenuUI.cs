using CitrioN.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuUI : MonoBehaviour
{
    public static LevelMenuUI Instance { get; private set; }

    [SerializeField] private Button[] levelButtons; // Drag your 4 buttons here
    [SerializeField] private LevelsDatabase levelsDatabase; // Drag your ScriptableObject here
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
        SetupLevelButtons();
    }

    private void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = SaveSystem.IsLevelUnlocked(i);
            var btn = levelButtons[i];
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();

            if (i < levelsDatabase.levels.Count)
            {
                var levelData = levelsDatabase.levels[i];
                text.text = $"{levelData.name} - 0 / {levelData.kittens}";
            }

            btn.interactable = unlocked;
            // No need to change sprite manually!
        }
    }


    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void OpenLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(levelIndex);
    }
}
