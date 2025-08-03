using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerRecorder : MonoBehaviour
{
    public float recordDuration = 5f;
    public GameObject clonePrefab;

    public float fadeDuration = 2f;
    public float unFadeDuration = 2f;

    [SerializeField] private KittyController playerMovement;

    private List<List<PlayerFrameData>> rewindSegments = new List<List<PlayerFrameData>>();
    private List<PlayerFrameData> currentSegment = new List<PlayerFrameData>();

    private float timer = 0f;

    public bool isRewinding = false;
    private bool allowRecording = true;
    private int rewindIndex;

    [SerializeField] TimeManager timeManager;
    [SerializeField] AudioSource normalMusic;
    [SerializeField] AudioSource rewindMusic;
    [SerializeField] MinimapTracker minimapTracker;
    [SerializeField] SwingInteractable swing;
    public Volume postProcessing;
    
    private Rigidbody rb;

    private int clonesUsed = 0;

    [SerializeField] TMP_Text clonesText;

    IEnumerator PreloadAudio()
    {
        rewindMusic.volume = 0f;
        rewindMusic.Play();
        yield return new WaitForSeconds(0.1f);
        rewindMusic.Stop();
        rewindMusic.volume = 1f;
    }

    void Start()
    {
        clonesUsed = 0;
        clonesText.text = "0/9";
        StartCoroutine(PreloadAudio());
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
                interactionPressed = Input.GetKeyDown(KeyCode.E),
                inventorySnapshot = new List<string>(Inventory.Instance.items.ConvertAll(i => i.itemName)),
                cameraPosition = playerMovement.cameraRoot.position,
                cameraRotation = playerMovement.cameraRoot.rotation
            });

            if (timer > recordDuration)
            {
                currentSegment.RemoveAt(0);
            }
        }
    }

    public void TriggerRewind()
    {
        if (!isRewinding && currentSegment.Count > 0 && clonesUsed < 9)
        {
            swing.currentAmplitude = 0;
            StartCoroutine(HandleRewind());
        }
    }
    private IEnumerator HandleRewind()
    {
        isRewinding = true;

        float fadeDuration = 2f;
        float timer = 0f;

        postProcessing.weight = 0;

        rewindMusic.pitch = 0f;
        rewindMusic.volume = 0f;
        yield return null;
        rewindMusic.Play();

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            normalMusic.pitch = Mathf.Lerp(1f, 0f, t);
            normalMusic.volume = Mathf.Lerp(1f, 0f, t);

            rewindMusic.pitch = Mathf.Lerp(0f, 1f, t);
            rewindMusic.volume = Mathf.Lerp(0f, 1f, t);

            playerMovement.speedMultiplier = Mathf.Lerp(1f, 0f, t);
            timeManager.decreaseTimeMultiplier = Mathf.Lerp(1f, 0f, t);

            postProcessing.weight = Mathf.Lerp(0f, 2f, t);

            timer += Time.deltaTime;
            yield return null;
        }

        postProcessing.weight = 2;
        normalMusic.pitch = 0f;
        normalMusic.volume = 0f;
        rewindMusic.pitch = 1f;
        rewindMusic.volume = 1f;
        normalMusic.Stop();

        // Now disable player control & physics for rewind playback
        allowRecording = false;
        playerMovement.enabled = false;
        rb.isKinematic = true;
        timeManager.decreaseTime = false;
        playerMovement.speedMultiplier = 0f;
        timeManager.decreaseTimeMultiplier = 0f;

        playerMovement.runSound.pitch = 0f;

        DialogueManager.Instance.shouldStart = false;

        float timeToRestore = timeManager.timeLimit - timeManager.remainingTime;
        // Play rewind frames with manual camera update
        for (int i = currentSegment.Count - 1; i >= 0;)
        {
            if (Time.timeScale != 0)
            {
                timeManager.remainingTime += timeToRestore * 4 / currentSegment.Count;
                PlayerFrameData frame = currentSegment[i];

                playerMovement.cameraRoot.position = frame.cameraPosition;
                playerMovement.cameraRoot.rotation = frame.cameraRotation;
                transform.position = frame.position;
                transform.rotation = frame.rotation;

                i -= 4; // Move to previous frame only if not paused
            }

            yield return new WaitForEndOfFrame(); // Always yield so Unity doesn't freeze
        }


        // Save rewind segment, spawn clones, etc.
        List<PlayerFrameData> copiedSegment = new List<PlayerFrameData>(currentSegment);
        rewindSegments.Add(copiedSegment);

        // Fade back from rewindMusic to normalMusic, restoring speeds & time decrease
        timer = 0f;
        normalMusic.pitch = 0f;
        normalMusic.volume = 0f;
        normalMusic.Play();

        transform.SetParent(null); // Ensure not parented to swing before replay
        KittyController.instance.isOnSwing = false;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            rewindMusic.pitch = Mathf.Lerp(1f, 0f, t);
            rewindMusic.volume = Mathf.Lerp(1f, 0f, t);

            normalMusic.pitch = Mathf.Lerp(0f, 1f, t);
            normalMusic.volume = Mathf.Lerp(0f, 1f, t);

            playerMovement.speedMultiplier = Mathf.Lerp(0f, 1f, t);
            timeManager.decreaseTimeMultiplier = Mathf.Lerp(0f, 1f, t);

            postProcessing.weight = Mathf.Lerp(2f, 0f, t);

            timer += Time.deltaTime;
            yield return null;
        }

        rewindMusic.Stop();

        // Finalize exact values
        normalMusic.pitch = 1f;
        normalMusic.volume = 1f;
        playerMovement.speedMultiplier = 1f;
        timeManager.decreaseTimeMultiplier = 1f;
        timeManager.decreaseTime = true;
        postProcessing.weight = 0;

        // Re-enable movement & physics
        playerMovement.enabled = true;
        rb.isKinematic = false;
        currentSegment.Clear();
        timer = 0f;
        allowRecording = true;
        DialogueManager.Instance.shouldStart = true;

        foreach (var segment in rewindSegments)
        {
            if (segment.Count > 0)
            {
                GameObject clone = Instantiate(clonePrefab, segment[0].position, segment[0].rotation);
                KittyClone cloneScript = clone.GetComponent<KittyClone>();
                cloneScript.Init(new List<PlayerFrameData>(segment));
                minimapTracker.AddCloneIcon(clone);

                yield return null; // wait a frame to spread the load
            }
        }
        clonesUsed += 1;
        clonesText.text = $"{clonesUsed}/9";
        if(clonesUsed == 9)
        {
            clonesText.color = new Color(0.8f, 0.2f, 0.2f);
        }
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
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    public List<string> inventorySnapshot; // Store item names only
}
