using UnityEngine;

public class SwingTriggerZone : MonoBehaviour
{
    public bool isAttachZone = true; // Whether this trigger attaches or detaches

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<KittyController>();
        if (player != null)
        {
            if (isAttachZone)
            {
                if (!KittyController.instance.isOnSwing && KittyController.instance.CanReattachToSwing())
                {
                    player.AttachToSwing();
                }
            }
            else
                player.JumpOffSwing(); // Or DetachFromSwing()
        }
    }
}
