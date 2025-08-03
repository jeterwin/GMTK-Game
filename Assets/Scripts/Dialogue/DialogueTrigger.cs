using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogue;
    public DialogueData toolDialogue;

    private bool playerInRange = false;
    private bool hasTriggered = false;

    void Update()
    {
        if (playerInRange && !hasTriggered && !DialogueManager.Instance.IsDialogueActive && !KittyController.instance.isOnSwing)
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
            hasTriggered = false;
            playerInRange = false;
    }
}
