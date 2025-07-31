using UnityEngine;

public class TimeRewindManager : MonoBehaviour
{
    public GameObject kittyClonePrefab;
    public PlayerRecorder recorder;
    public Transform kittyPlayerTransform; // Reference to the real player

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Trigger rewind
        {
            SpawnClone();
        }
    }

    void SpawnClone()
    {
        // Spawn clone at the kitty's current position and rotation
        GameObject clone = Instantiate(
            kittyClonePrefab,
            kittyPlayerTransform.position,
            kittyPlayerTransform.rotation
        );

        // Feed the recent recorded data to the clone
        KittyClone cloneScript = clone.GetComponent<KittyClone>();
        cloneScript.Init(recorder.GetReplayData());
    }
}


