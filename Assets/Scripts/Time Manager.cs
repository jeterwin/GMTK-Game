using UnityEngine;
using System.Collections;
using TMPro; // Ensure you have TextMeshPro package installed

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float timeLimit;
    private float remainingTime;
    private float countdownTime = 3f;
    public TMP_Text timeText;
    public TMP_Text countdownText; // Reference to the TextMeshProUGUI component for countdown display
    public GameObject HUDCanvas;
    public GameObject countdownCanvas;
    public Animator timeAnimator; // Reference to the Animator component for animations
    public Animator countdownAnimator; // Animator for handling text animations
    private float flashInterval = 10f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownText.text = countdownTime.ToString("F1") + "s"; // Display initial countdown time with 1 decimal place   
        timeText.text = timeLimit.ToString("F2") + "s"; // Display initial time limit with 2 decimal places
        StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            timeText.text = remainingTime.ToString("F2") + "s"; // Display remaining time with 2 decimal places
            if(remainingTime > 40) {
                flashInterval = 10f; // Flash every 10 seconds for more than 30 seconds remaining
            }
            else if (remainingTime > 20)
            {
                flashInterval = 5f; // Flash every 5 seconds for more than 10 seconds remaining
            }else if (remainingTime > 10)
            {
                flashInterval = 1f; // Flash every 2 seconds for more than 5 seconds remaining
            }
            else
            {
                flashInterval = .5f; // Flash every second for less than or equal to 5 seconds remaining
            }

            if (Mathf.Abs((remainingTime % flashInterval)) < 0.05f)
            {
                timeAnimator.Play("Time Flash", -1, 0);
            }
            timeText.color = GetTimeColor(remainingTime, timeLimit); // Update text color based on remaining time
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
