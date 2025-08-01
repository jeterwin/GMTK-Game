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
        newGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
