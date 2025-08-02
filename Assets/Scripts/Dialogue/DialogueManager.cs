using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public bool IsDialogueActive => dialogueBox.activeSelf;

    private Queue<DialogueLine> lineQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private DialogueLine currentLine;
    private Coroutine typingCoroutine;

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
        dialogueBox.SetActive(true);
        lineQueue.Clear();

        foreach (var line in data.lines)
            lineQueue.Enqueue(line);

        KittyController.instance.enabled = false; // Assuming you have a singleton PlayerController
        DisplayNextLine();
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
            typingCoroutine = StartCoroutine(TypeText(currentLine.missingItemText));
        }
        else
        {
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
        KittyController.instance.enabled = true;
    }

    private void Update()
    {
        if (dialogueBox.activeSelf && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            DisplayNextLine();
        }
    }
}

