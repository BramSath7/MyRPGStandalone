using System.Collections.Generic;
using UnityEngine;

public class Object_Character : MonoBehaviour, IDamageable {
    public Unit Runtime;
    public Rigidbody2D rb;
    public Animator anim;
    public AudioSource walk;
    public AudioSource Ability;
    public Vector2 FaceDir;
    public Vector2 InputDir;
    public bool IsPlayer => Runtime?.IsPlayer ?? false;
    public List<AbilityCol> abilityCol;
    public Dictionary<string, AbilityCol> colliderMap = new();
    // 
    public float Udt { get; set; }
    public float Fixed {  get; set; }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        walk = GetComponent<AudioSource>();
        Ability = transform.Find("Ability").GetComponent<AudioSource>();
    }
    public void Bind(Unit u) {
        Runtime = u;
        if (Runtime == null) return;
        Runtime.LoadedUnit = this;
    }
    public void OnCombatStart() {
        NotifyTime.OnTick += OnUpdate;
        NotifyTime.OnFix += OnFixed;
        if (Runtime.IsPlayer) {
            GameNotify.OnMove += InputMove;
        }
    }
    private void OnDestroy() {
        NotifyTime.OnTick -= OnUpdate;
        NotifyTime.OnFix -= OnFixed;
        GameNotify.OnMove -= InputMove;
        Runtime = null;
    }
    private void InputMove(Vector2 dir) => InputDir = dir;
    //
    private void OnUpdate(float dt) {
        if (Runtime == null) return;
        Runtime.Update(dt);
        if (Runtime.IsAlive) {
            Udt += dt;
        } else {
            return;
        }
    }
    private void OnFixed(float fdt) {
        Fixed = fdt;
        if (!Runtime.IsAI) {
            MoveTo(InputDir);
        }
    }
    public void MoveTo(Vector2 dir) {
        if (dir.sqrMagnitude > 0.001f) {
            FaceDir = dir.normalized;
            Vector2 newPos = rb.position + FaceDir * 5f * Fixed;
            rb.MovePosition(newPos);
            Runtime.CurrentState = CharState.Moving;
        } else {
            Runtime.CurrentState = CharState.Idle;
        }
        HandleAnim(FaceDir);
    }
    //
    public void HandleAnim(Vector2 newDir, string trigger = null) {
        anim.SetFloat("MoveX", newDir.x);
        anim.SetFloat("MoveY", newDir.y);
        //
        anim.SetBool("IsMoving", Runtime.CurrentState == CharState.Moving);
        anim.SetBool("IsDead", Runtime.CurrentState == CharState.Died);
        anim.SetBool("IsCasting", Runtime.CurrentState == CharState.Casting);
        if (!string.IsNullOrEmpty(trigger)) {
            anim?.SetTrigger(trigger);
        }
    }
    // 
    public void ApplyDamage(float damage) {

    }
    public void ApplyBuff(BuffInst eff) => Runtime.Buff.Add(eff);
}

[System.Serializable]
public class AbilityCol {
    public string Group;
    public List<Hitbox_Ability> Hitboxes = new();
}