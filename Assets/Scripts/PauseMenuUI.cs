using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private static PauseMenuUI instance;
    public static PauseMenuUI Instance => instance;

    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            
            Destroy(gameObject);
            return;
        }
        instance = this;

        resumeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Hide();
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionsMenuUI.Instance.Show();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // Reset time before changing scenes
            SceneManager.LoadScene(0);
        });

        Hide(); // Make sure pause menu is hidden at start
    }

    

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
