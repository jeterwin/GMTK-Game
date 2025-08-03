using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenuUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        //Hide();
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
