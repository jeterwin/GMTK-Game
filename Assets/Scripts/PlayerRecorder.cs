using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    public float recordDuration = 5f;
    public GameObject clonePrefab;

    [SerializeField] private KittyController playerMovement;

    private List<List<PlayerFrameData>> rewindSegments = new List<List<PlayerFrameData>>();
    private List<PlayerFrameData> currentSegment = new List<PlayerFrameData>();

    private float timer = 0f;

    private bool isRewinding = false;
    private bool allowRecording = true;
    private int rewindIndex;

    [SerializeField] TimeManager timeManager;
    [SerializeField] AudioSource normalMusic;
    [SerializeField] AudioSource rewindMusic;
    [SerializeField] MinimapTracker minimapTracker;

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

            currentSegment.Add(new PlayerFrameData
            {
                position = transform.position,
                rotation = transform.rotation,
                animationState = GetCurrentAnimationState(),
                interactionPressed = Input.GetKeyDown(KeyCode.E)
            });

            if (timer > recordDuration)
            {
                currentSegment.RemoveAt(0);
            }
        }
    }

    public void TriggerRewind()
    {
        if (!isRewinding && currentSegment.Count > 0)
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

        for (int i = currentSegment.Count - 1; i >= 0; i--)
        {
            PlayerFrameData frame = currentSegment[i];
            transform.position = frame.position;
            transform.rotation = frame.rotation;
            yield return new WaitForEndOfFrame();
        }

        // Save this segment
        List<PlayerFrameData> copiedSegment = new List<PlayerFrameData>(currentSegment);
        rewindSegments.Add(copiedSegment);

        normalMusic.Play();
        rewindMusic.Stop();
        timeManager.remainingTime = timeManager.timeLimit;
        timeManager.decreaseTime = true;

        foreach (var segment in rewindSegments)
        {
            if (segment.Count > 0)
            {
                GameObject clone = Instantiate(clonePrefab, segment[0].position, segment[0].rotation);
                KittyClone cloneScript = clone.GetComponent<KittyClone>();
                cloneScript.Init(new List<PlayerFrameData>(segment)); // make a defensive copy
                // Spawn clone to replay original movements
                minimapTracker.AddCloneIcon(clone);
            }
        };

        // Resume control
        currentSegment.Clear();
        timer = 0f;
        rb.isKinematic = false;
        playerMovement.enabled = true;
        allowRecording = true;
        isRewinding = false;
    }

    public List<PlayerFrameData> GetReplayData()
    {
        return new List<PlayerFrameData>(currentSegment);
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
