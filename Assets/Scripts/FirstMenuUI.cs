using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstMenuUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        continueButton.gameObject.SetActive(SaveSystem.HasGameStarted());

        newGameButton.onClick.AddListener(() =>
        {
            SaveSystem.InitNewGame();

            // Load first level or level select menu
            // Example: SceneManager.LoadScene("LevelSelect");
        });

        continueButton.onClick.AddListener(() =>
        {
            LevelMenuUI.Instance.Show();
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionsMenuUI.Instance.Show();
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
