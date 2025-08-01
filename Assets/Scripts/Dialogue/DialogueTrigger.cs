using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogue;

    private bool playerInRange = false;
    private bool hasTriggered = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !hasTriggered && !DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            hasTriggered = true; // prevent restart
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
