using UnityEngine;
using UnityEngine.UI;

public class MinimapTracker : MonoBehaviour
{
    public Transform player;                      // Player to track
    public RectTransform minimapRect;             // The minimap UI image
    public RectTransform icon;                    // The player icon
    public Vector2 worldMin;                      // Bottom-left corner of world area
    public Vector2 worldMax;                      // Top-right corner of world area

    void Update()
    {
        if (!player || !minimapRect || !icon) return;

        Vector2 worldPos = new Vector2(player.position.x, player.position.z); // Use XZ plane

        // Assume worldMin and worldMax are bottom-left and top-right corners
        float normalizedX = Mathf.InverseLerp(worldMax.x, worldMin.x, worldPos.x);
        float normalizedY = Mathf.InverseLerp(worldMax.y, worldMin.y, worldPos.y); // use Z for Y

        Vector2 minimapSize = minimapRect.rect.size;

        // Offset from center
        icon.anchoredPosition = new Vector2(
            normalizedX * minimapSize.x,
            normalizedY * minimapSize.y
        );

        icon.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);

    }
}
