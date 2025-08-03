using TMPro;
using UnityEngine;
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
            if (i >= levelsDatabase.levels.Count) continue;

            var levelData = levelsDatabase.levels[i];
            var textComponent = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = $"{levelData.name} - 0 / {levelData.kittens}";
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
