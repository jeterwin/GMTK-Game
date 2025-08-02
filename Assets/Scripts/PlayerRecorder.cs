using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool isRewinding = false;
    private bool allowRecording = true;
    private int rewindIndex;

    [SerializeField] TimeManager timeManager;
    [SerializeField] AudioSource normalMusic;
    [SerializeField] AudioSource rewindMusic;
    [SerializeField] MinimapTracker minimapTracker;
    public GreyscaleFade greyscaleFade;
    
    private Rigidbody rb;
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
        if (!isRewinding && currentSegment.Count > 0)
        {
            StartCoroutine(HandleRewind());
        }
    }
    private IEnumerator HandleRewind()
    {
        isRewinding = true;

        float fadeDuration = 2f;
        float timer = 0f;

        rewindMusic.pitch = 0f;
        rewindMusic.volume = 0f;
        yield return null;
        rewindMusic.Play();

        if (greyscaleFade != null)
        {
            greyscaleFade.FadeToGrayscale(fadeDuration);
        }

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            normalMusic.pitch = Mathf.Lerp(1f, 0f, t);
            normalMusic.volume = Mathf.Lerp(1f, 0f, t);

            rewindMusic.pitch = Mathf.Lerp(0f, 1f, t);
            rewindMusic.volume = Mathf.Lerp(0f, 1f, t);

            playerMovement.speedMultiplier = Mathf.Lerp(1f, 0f, t);
            timeManager.decreaseTimeMultiplier = Mathf.Lerp(1f, 0f, t);

            timer += Time.deltaTime;
            yield return null;
        }

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

        float timeToRestore = timeManager.timeLimit - timeManager.remainingTime;
        // Play rewind frames with manual camera update
        for (int i = currentSegment.Count - 1; i >= 0; i-=2)
        {
            timeManager.remainingTime += timeToRestore * 2 / currentSegment.Count;
            PlayerFrameData frame = currentSegment[i];
            playerMovement.cameraRoot.position = frame.cameraPosition;
            playerMovement.cameraRoot.rotation = frame.cameraRotation;
            transform.position = frame.position;
            transform.rotation = frame.rotation;
            yield return new WaitForEndOfFrame();
        }

        // Save rewind segment, spawn clones, etc.
        List<PlayerFrameData> copiedSegment = new List<PlayerFrameData>(currentSegment);
        rewindSegments.Add(copiedSegment);

        // Fade back from rewindMusic to normalMusic, restoring speeds & time decrease
        timer = 0f;
        normalMusic.pitch = 0f;
        normalMusic.volume = 0f;
        normalMusic.Play();

        if (greyscaleFade != null)
        {
            greyscaleFade.FadeToColor(fadeDuration);
        }

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            rewindMusic.pitch = Mathf.Lerp(1f, 0f, t);
            rewindMusic.volume = Mathf.Lerp(1f, 0f, t);

            normalMusic.pitch = Mathf.Lerp(0f, 1f, t);
            normalMusic.volume = Mathf.Lerp(0f, 1f, t);

            playerMovement.speedMultiplier = Mathf.Lerp(0f, 1f, t);
            timeManager.decreaseTimeMultiplier = Mathf.Lerp(0f, 1f, t);

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

        // Re-enable movement & physics
        playerMovement.enabled = true;
        rb.isKinematic = false;
        currentSegment.Clear();
        timer = 0f;
        allowRecording = true;
        isRewinding = false;

        yield return StartCoroutine(SpawnClonesCoroutine());
        isRewinding = false;
    }

    private IEnumerator SpawnClonesCoroutine()
    {
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
