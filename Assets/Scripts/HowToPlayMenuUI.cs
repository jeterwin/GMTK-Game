using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HowToPlayMenuUI : MonoBehaviour
{
    public static HowToPlayMenuUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI howToPlayText; // 🆕 Drag your TMP UI text here in the Inspector
    [TextArea(5, 20)] [SerializeField] private string fullText; // 🆕 Optional: editable full text

    [SerializeField] private float typingSpeed = 0.02f; // 🆕 Typing speed in seconds per character

    private Coroutine typewriterCoroutine;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypeText());
    }

    public void Hide()
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        gameObject.SetActive(false);
    }

    private IEnumerator TypeText()
    {
        howToPlayText.text = "";
        foreach (char c in fullText)
        {
            howToPlayText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
