using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    public float recordDuration = 5f;
    public GameObject clonePrefab;

    [SerializeField] private KittyController playerMovement;
    private List<PlayerFrameData> frames = new List<PlayerFrameData>();
    private float timer = 0f;

    private bool isRewinding = false;
    private bool allowRecording = true;
    private int rewindIndex;

    [SerializeField] TimeManager timeManager;
    [SerializeField] AudioSource normalMusic;
    [SerializeField] AudioSource rewindMusic;

    private Rigidbody rb;

    void Start()
    {
        recordDuration = TimeManager.Instance.timeLimit;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (allowRecording && !isRewinding)
        {
            timer += Time.deltaTime;

            frames.Add(new PlayerFrameData
            {
                position = transform.position,
                rotation = transform.rotation,
                animationState = GetCurrentAnimationState(),
                interactionPressed = Input.GetKeyDown(KeyCode.E)
            });

            if (timer > recordDuration)
            {
                frames.RemoveAt(0);
            }
        }
    }

    public void TriggerRewind()
    {
        if (!isRewinding && frames.Count > 0)
        {
            StartCoroutine(HandleRewind());
        }
    }

    private IEnumerator HandleRewind()
    {
        isRewinding = true;
        allowRecording = false;
        playerMovement.enabled = false;
        rb.isKinematic = true;
        timeManager.decreaseTime = false;
        rewindMusic.Play();
        normalMusic.Stop();

        // Rewind player visually
        for (rewindIndex = frames.Count - 1; rewindIndex >= 0; rewindIndex-=4)
        {
            timeManager.remainingTime += (timeManager.timeLimit - timeManager.remainingTime)*4/frames.Count;
            PlayerFrameData frame = frames[rewindIndex];
            transform.position = frame.position;
            transform.rotation = frame.rotation;
            yield return null;
        }

        normalMusic.Play();
        rewindMusic.Stop();
        timeManager.remainingTime = timeManager.timeLimit;
        timeManager.decreaseTime = true;
        // Spawn clone to replay original movements
        GameObject clone = Instantiate(clonePrefab, frames[0].position, frames[0].rotation);
        KittyClone cloneScript = clone.GetComponent<KittyClone>();
        cloneScript.Init(new List<PlayerFrameData>(frames));

        // Resume control
        frames.Clear();
        timer = 0f;
        rb.isKinematic = false;
        playerMovement.enabled = true;
        allowRecording = true;
        isRewinding = false;
    }

    public List<PlayerFrameData> GetReplayData()
    {
        return new List<PlayerFrameData>(frames);
    }

    string GetCurrentAnimationState()
    {
        return ""; // Optional
    }
}

[System.Serializable]
public struct PlayerFrameData
{
    public Vector3 position;
    public Quaternion rotation;
    public string animationState;
    public bool interactionPressed;
}
