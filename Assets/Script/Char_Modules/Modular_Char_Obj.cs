using UnityEngine;

public class Module_Char_Obj : MonoBehaviour, IDamageable {
    public Char_Runtime Runtime;
    public Animator anim;
    public AudioSource walk;
    public AudioSource ability;
    public Vector2 CurrentDir {  get; private set; }
    public bool IsPlayer => Runtime.IsPlayer;
    private void Awake()
    {
        
    }
    public void OnObjectStart(Char_Runtime runtime)
    {
        Runtime = runtime;
        NotifyTime.OnTick += OnUpdate;
    }
    public void SetDirection(Vector2 dir)
    {
        CurrentDir = dir.normalized;
    }
    public void OnObjectEnd() { }
    public void OnUpdate(float dt)
    {
        
    }
    public void ApplyDamage(float dmg) { }
}

