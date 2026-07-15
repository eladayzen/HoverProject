using UnityEngine;

[AddComponentMenu("Ball Roller/Kill Zone")]
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var hoverPlayer = other.GetComponentInParent<Dreamteck.Forever.HoverPlayer>();
        if (hoverPlayer == null) return;
        Dreamteck.Forever.EndScreen.Open();
    }
}
