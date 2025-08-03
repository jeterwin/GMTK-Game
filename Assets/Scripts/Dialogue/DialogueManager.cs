using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public AudioSource momMeow;
    public AudioSource kittenMeow;

    public bool IsDialogueActive => dialogueBox.activeSelf;

    private Queue<DialogueLine> lineQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private DialogueLine currentLine;
    private Coroutine typingCoroutine;
    public bool shouldStart = true;

    private Coroutine displayCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void StartDialogue(DialogueData data)
    {
        if (!shouldStart) return;

        // Stop current dialogue progression if it's running
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        dialogueText.text = "";

        dialogueBox.SetActive(true);
        lineQueue.Clear();

        foreach (var line in data.lines)
            lineQueue.Enqueue(line);

        displayCoroutine = StartCoroutine(DisplayDialogue());
    }



    private IEnumerator DisplayDialogue()
    {
        while (lineQueue.Count > 0)
        {
            DisplayNextLine();
            yield return new WaitForSeconds(3);
        }
        DisplayNextLine();
        displayCoroutine = null;
    }

    public void DisplayNextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentLine.text;
            isTyping = false;
            return;
        }

        if (lineQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lineQueue.Dequeue();
        nameText.text = currentLine.speakerName;
        portraitImage.sprite = currentLine.portrait;

        if (!string.IsNullOrEmpty(currentLine.requiredItemName) &&
            !Inventory.Instance.HasItem(currentLine.requiredItemName))
        {
            momMeow.Play();
            typingCoroutine = StartCoroutine(TypeText(currentLine.missingItemText));
        }
        else
        {
            if (currentLine.meowerType == MeowerType.Mom)
            {
                momMeow.Play();
            }
            else
            {
                kittenMeow.Play();
            }
            typingCoroutine = StartCoroutine(TypeText(currentLine.text));
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
    }
}

