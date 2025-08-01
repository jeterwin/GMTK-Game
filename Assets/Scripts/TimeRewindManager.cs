using UnityEngine;

using UnityEngine;

public class TimeRewindManager : MonoBehaviour
{
    public PlayerRecorder recorder;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            recorder.TriggerRewind();
        }
    }
}