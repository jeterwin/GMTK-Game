using UnityEngine;

public class LevelMenuUI : MonoBehaviour
{
    public static LevelMenuUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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
