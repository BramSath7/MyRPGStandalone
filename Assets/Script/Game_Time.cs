using UnityEngine;

public class Game_Time : MonoBehaviour {
    // Update is called once per frame
    void Update() {
        NotifyTime.Tick(Time.deltaTime);
    }
    private void FixedUpdate() {
        NotifyTime.Fix(Time.fixedDeltaTime);
    }
    private void LateUpdate() {
        NotifyTime.Late(Time.deltaTime);
    }
}

public static class NotifyTime
{
    public static event System.Action<float> OnTick;
    public static event System.Action<float> OnFix;
    public static event System.Action<float> OnLate;
    public static void Tick(float time)
        => OnTick?.Invoke(time);
    public static void Fix(float time)
        => OnFix?.Invoke(time);
    public static void Late(float time)
        => OnLate?.Invoke(time);
    //
    public static bool Update(ref float timer, float dt) {
        if (timer <=0) return false;
        timer -= dt;
        if (timer <= 0) {
            timer = 0;
            return true;
        }
        return false;
    }
}
