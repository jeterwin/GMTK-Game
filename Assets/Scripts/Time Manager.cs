using UnityEngine;
using System.Collections;
using TMPro; // Ensure you have TextMeshPro package installed

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float timeLimit;
    public float remainingTime;
    [SerializeField] float countdownTime = 3f;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text countdownText; // Reference to the TextMeshProUGUI component for countdown display
    [SerializeField] GameObject HUDCanvas;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] Animator timeAnimator; // Reference to the Animator component for animations
    [SerializeField] Animator countdownAnimator; // Animator for handling text animations
    [SerializeField] float flashInterval = 10f;
    public bool decreaseTime;
    [SerializeField] AudioSource music;
    public float decreaseTimeMultiplier = 1f;
    [SerializeField] GameObject loseScreen;
    [SerializeField] PlayerRecorder playerRecorder;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLimit = LevelManager.Instance.timeLimit;
        remainingTime = timeLimit;
        decreaseTime = false;
        countdownText.text = countdownTime.ToString("F1") + "s"; // Display initial countdown time with 1 decimal place   
        timeText.text = timeLimit.ToString("F2") + "s"; // Display initial time limit with 2 decimal places
        StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (decreaseTime)
        {
            remainingTime -= Time.deltaTime * decreaseTimeMultiplier;
        }
        if (remainingTime > 0 )
        {
            if (decreaseTime)
            {
                remainingTime -= Time.deltaTime;
            }
            timeText.text = remainingTime.ToString("F2") + "s"; // Display remaining time with 2 decimal places
                                                                // Adjust music speed (pitch) based on remaining time
            if (remainingTime > 40)
            {
                music.pitch = 1f;   // Normal speed
                flashInterval = 10f;
            }
            else if (remainingTime > 20)
            {
                music.pitch = 1.2f; // Slightly faster
                flashInterval = 5f;
            }
            else if (remainingTime > 10)
            {
                music.pitch = 1.4f;
                flashInterval = 1f;
            }
            else
            {
                if (remainingTime > 5)
                {
                    music.pitch = 1.6f; // Fast and tense
                }
                else
                {
                    music.pitch = 2f; // Very fast and tense
                }
                flashInterval = 0.5f;
            }


            if (Mathf.Abs((remainingTime % flashInterval)) < 0.05f)
            {
                timeAnimator.Play("Time Flash", -1, 0);
            }
            timeText.color = GetTimeColor(remainingTime, timeLimit); // Update text color based on remaining time
        }
        else
        {
            if (!playerRecorder.isRewinding)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                loseScreen.SetActive(true); // Show lose screen when time runs out
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        while(countdownTime > 0)
        {
            yield return new WaitForSeconds(.1f);
            countdownTime -= .1f;
            countdownText.text = countdownTime.ToString("F1") + "s"; // Display countdown with 1 decimal place
            if(countdownTime >= 1 && Mathf.Abs(countdownTime % 1) < 0.1)
            {
                countdownAnimator.Play("Time Flash", -1, 0); // Play countdown animation if needed
            }
        }
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the game
        countdownCanvas.SetActive(false); // Hide the countdown canvas
        HUDCanvas.SetActive(true); // Show the HUD canvas
        remainingTime = timeLimit;
        decreaseTime = true;
    }

    Color GetTimeColor(float remainingTime, float totalTime)
    {
        float t = Mathf.Clamp01(remainingTime / totalTime);

        Color naturalGreen = new Color(0.2f, 0.6f, 0.2f);      // Softer, muted green
        Color naturalYellow = new Color(1f, 0.85f, 0.3f);      // Warm golden yellow
        Color naturalRed = new Color(0.8f, 0.2f, 0.2f);         // Muted red with slight brownish tone

        if (t > 0.5f)
        {
            float lerp = (t - 0.5f) * 2f; // Normalize to 0–1
            return Color.Lerp(naturalYellow, naturalGreen, lerp);
        }
        else
        {
            float lerp = t * 2f; // Normalize to 0–1
            return Color.Lerp(naturalRed, naturalYellow, lerp);
        }

    }
}
