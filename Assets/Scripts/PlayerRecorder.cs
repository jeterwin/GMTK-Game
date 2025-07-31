using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    public float recordDuration = 5f;

    private List<PlayerFrameData> frames = new List<PlayerFrameData>();
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // Capture state
        frames.Add(new PlayerFrameData
        {
            position = transform.position,
            rotation = transform.rotation,
            animationState = GetCurrentAnimationState(), // Your method
            interactionFlags = CheckInteractionsThisFrame() // Custom logic
        });

        // Limit recording to a certain time window
        if (timer > recordDuration)
        {
            frames.RemoveAt(0);
        }
    }

    public List<PlayerFrameData> GetReplayData()
    {
        return new List<PlayerFrameData>(frames);
    }

    // Example method signatures
    string GetCurrentAnimationState() { return ""; }
    bool CheckInteractionsThisFrame() { return false; }
}

[System.Serializable]
public struct PlayerFrameData
{
    public Vector3 position;
    public Quaternion rotation;
    public string animationState;
    public bool interactionFlags; // You can expand this for more complex data
}
