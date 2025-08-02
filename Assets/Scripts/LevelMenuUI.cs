using UnityEngine;
using UnityEngine.UI;

public class LevelMenuUI : MonoBehaviour
{
    public static LevelMenuUI Instance { get; private set; }

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    
    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
