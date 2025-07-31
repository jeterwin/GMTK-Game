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

    private IEnumerator PlayReplay()
    {
        while (currentFrame < playbackData.Count)
        {
            var frame = playbackData[currentFrame];
            transform.position = frame.position;
            transform.rotation = frame.rotation;
            PlayAnimation(frame.animationState);

            if (frame.interactionFlags)
            {
                TriggerInteraction();
            }

            currentFrame++;
            yield return new WaitForEndOfFrame(); // or WaitForSeconds for slow motion
        }

        Destroy(gameObject); // Or let the clone idle
    }

    void PlayAnimation(string animState) {
        // Your Animator handling
    }

    void TriggerInteraction() {
        // Call the same interaction system (e.g., press button)
    }
}
