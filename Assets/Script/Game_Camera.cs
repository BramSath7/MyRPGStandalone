using UnityEngine;

public class Game_Camera : MonoBehaviour
{
    public static Game_Camera Instance;
    public Vector2 minRange;
    public Vector2 maxRange;
    public Transform Target;
    public float minSpeed = 3f;
    public float maxSpeed = 20f;
    public float maxDist = 10f;
    private void Awake() {
        Instance = this;
    }
    private void OnDestroy()
    {
        NotifyTime.OnLate -= OnLateUpdate;
    }
    public void SetRange(Vector2 min, Vector2 max) {
        minRange = min;
        maxRange = max;
    }
    public void SetTarget(Transform tgt)
    {
        Target = tgt;
        NotifyTime.OnLate += OnLateUpdate;
    }
    public void OnLateUpdate(float dt) {
        if (Target == null) return;

        Vector3 desired = Target.position;
        desired.z = transform.position.z;
        float dist = Vector3.Distance(transform.position, desired);

        float t = Mathf.Clamp01(dist / maxDist);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, t);

        Vector3 pos = Vector3.MoveTowards(transform.position, desired, speed * Time.deltaTime);
        pos.x = Mathf.Clamp(pos.x, minRange.x, maxRange.x);
        pos.y = Mathf.Clamp(pos.y, minRange.y, maxRange.y);
        transform.position = pos;
    }
}
