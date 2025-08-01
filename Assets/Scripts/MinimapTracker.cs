using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapTracker : MonoBehaviour
{
    public Transform player;
    public RectTransform minimapRect;
    public RectTransform playerIcon;
    public RectTransform kittenIconPrefab;
    public RectTransform taskGlowPrefab;

    public Vector2 worldMin;
    public Vector2 worldMax;

    private List<GameObject> kittens = new List<GameObject>();
    private List<GameObject> tasks = new List<GameObject>();

    private Dictionary<GameObject, RectTransform> kittenIcons = new Dictionary<GameObject, RectTransform>();
    private Dictionary<GameObject, RectTransform> taskGlows = new Dictionary<GameObject, RectTransform>();

    public RectTransform playerIconPrefab;
    private RectTransform playerIconInstance;

    void Start()
    {
        playerIconInstance = Instantiate(playerIconPrefab, minimapRect);
        playerIconInstance.name = "PlayerIcon";
        playerIconInstance.anchorMin = playerIconInstance.anchorMax = playerIconInstance.pivot = new Vector2(.5f, .5f);

        kittens.AddRange(GameObject.FindGameObjectsWithTag("Kitten"));
        tasks.AddRange(GameObject.FindGameObjectsWithTag("Task"));

        foreach (var kitten in kittens)
        {
            var icon = Instantiate(kittenIconPrefab, minimapRect);
            icon.name = "KittenIcon_" + kitten.name;
            icon.anchorMin = icon.anchorMax = icon.pivot = new Vector2(.5f, .5f);
            kittenIcons[kitten] = icon;

            UpdateIconPosition(kitten.transform.position, icon);
        }

        foreach (var task in tasks)
        {
            var glow = Instantiate(taskGlowPrefab, minimapRect);
            glow.name = "TaskGlow_" + task.name;
            glow.anchorMin = glow.anchorMax = glow.pivot = new Vector2(.5f, .5f);
            taskGlows[task] = glow;

            UpdateIconPosition(task.transform.position, glow);
        }
    }

    void Update()
    {
        if (!player || !minimapRect || !playerIcon) return;

        Vector2 minimapSize = minimapRect.rect.size;

        Vector2 playerWorldPos = new Vector2(player.position.x, player.position.z);
        playerIconInstance.anchoredPosition = WorldToMinimapPosition(playerWorldPos, minimapSize);

        // Update kitten icons visibility and position
        foreach (var kvp in kittenIcons)
        {
            var go = kvp.Key;
            var icon = kvp.Value;

            bool hasTag = go != null && go.CompareTag("Kitten");
            icon.gameObject.SetActive(hasTag);

            if (hasTag)
                UpdateIconPosition(go.transform.position, icon);
        }

        // Update task glows visibility and position
        foreach (var kvp in taskGlows)
        {
            var go = kvp.Key;
            var glow = kvp.Value;

            bool hasTag = go != null && go.CompareTag("Task");
            glow.gameObject.SetActive(hasTag);

            if (hasTag)
                UpdateIconPosition(go.transform.position, glow);
        }
    }

    private void UpdateIconPosition(Vector3 worldPos3D, RectTransform icon)
    {
        Vector2 worldPos = new Vector2(worldPos3D.x, worldPos3D.z);
        Vector2 minimapSize = minimapRect.rect.size;
        Vector2 anchoredPos = WorldToMinimapPosition(worldPos, minimapSize);
        icon.anchoredPosition = anchoredPos;
    }

    private Vector2 WorldToMinimapPosition(Vector2 worldPos, Vector2 minimapSize)
    {
        float normalizedX = Mathf.InverseLerp(worldMax.x, worldMin.x, worldPos.x);
        float normalizedY = Mathf.InverseLerp(worldMax.y, worldMin.y, worldPos.y);

        return new Vector2(
            (normalizedX - 0.5f) * minimapSize.x,
            (normalizedY - 0.5f) * minimapSize.y
        );
    }
}
