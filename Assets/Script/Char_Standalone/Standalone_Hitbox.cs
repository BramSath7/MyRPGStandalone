using UnityEngine;

public class Hitbox_Ability : MonoBehaviour {
    public event System.Action<Collider2D> OnHit;
    public void OnTriggerEnter2D(Collider2D other) {
        OnHit?.Invoke(other);
    }
}
