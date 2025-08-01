using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittyClone : MonoBehaviour
{
    private List<PlayerFrameData> playbackData;
    private int currentFrame = 0;

    public void Init(List<PlayerFrameData> data)
    {
        playbackData = data;
        StartCoroutine(PlayReplay());
    }

    IEnumerator PlayReplay()
    {
        foreach (var frame in playbackData)
        {
            transform.position = frame.position;
            transform.rotation = frame.rotation;

            // Play animation if needed
            PlayAnimation(frame.animationState);

            // Trigger interaction if this frame recorded a press
            if (frame.interactionPressed)
            {
                TriggerInteraction(); // Define this method to call nearby IInteractables
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    void PlayAnimation(string animState) {
        // Your Animator handling
    }

    void TriggerInteraction()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject); // Pass the clone as the caller
            }
        }
    }
}
