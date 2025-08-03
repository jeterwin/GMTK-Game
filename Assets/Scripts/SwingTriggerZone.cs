using UnityEngine;

public class SwingTriggerZone : MonoBehaviour
{
    public bool isAttachZone = true; // Whether this trigger attaches or detaches
    [SerializeField] PlayerRecorder playerRecorder;

    void OnTriggerEnter(Collider other)
    {
        // Check the root object (not just the collider that hit)
        GameObject root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;

        Debug.Log($"Entered: {other.name}, root: {root.name}, tag: {root.tag}");

        if (!root.CompareTag("Player"))
            return;

        KittyController player = root.GetComponent<KittyController>();
        if (player == null) return;

        if (isAttachZone)
        {
            if (!player.isOnSwing && player.CanReattachToSwing() && !playerRecorder.isRewinding)
                player.AttachToSwing();
        }
        else
        {
            player.JumpOffSwing();
        }
    }

}
