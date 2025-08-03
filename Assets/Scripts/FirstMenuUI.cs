using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstMenuUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button howToButton;

    private void Awake()
    {
        continueButton.gameObject.SetActive(SaveSystem.HasGameStarted());

        newGameButton.onClick.AddListener(() =>
        {
            SaveSystem.InitNewGame();
            Loader.Load(Loader.Scene.Level1);
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

        howToButton.onClick.AddListener(() =>
        {
            HowToPlayMenuUI.Instance.Show();
        });
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Show LevelMenuUI after scene reload
        LevelMenuUI.Instance.Show();

        // Unsubscribe so it only runs once
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
