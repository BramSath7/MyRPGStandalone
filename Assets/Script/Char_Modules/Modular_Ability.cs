
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(menuName = "Data/Ability")]
public class Ability_Data : ScriptableObject
{
    [Header("Identity")]
    public string ID;
    public string Name;
    [TextArea] public string Desc;

    [Header("Modifier")]
    public float Cooldown;
    public float CastTime;
    public float Duration;
    [Header("Scale")]
    public float Scale = 1;
    [Header("AIM")]
    public float MinRange;
    public float MaxRange;
    [Header("Combo")]
    public int MaxCombo = 1;
    public float ComboWindow = 1.5f;
    public bool AutoCombo;
    public bool LoopCombo;
    [Header("Charge")]
    public float ChargeLimit;
    public int ChargeMax = 1;
    [Header("Stack")]
    public bool Stack;
    public int StackMax = 1;
    [Header("Visual")]
    public List<Sprite> Icons = new();
    [Header("Hitbox")]
    public AssetReferenceT<Hitbox_Data> Hitboxes; // AssetrefT
    public Level Level = new();
    //
    public List<AbilityAction> Actions;
}
[System.Serializable]
public class AbilityRuntime {
    public Ability_Data Data { get; }
    [System.NonSerialized] public Char_Runtime Owner;
    public AbilityRuntime(Ability_Data newData, Char_Runtime newOwner) {
        Data = newData; 
        Owner = newOwner;
    }
    public bool Holding { get; private set; }
    public bool IsCasting { get; private set; }
    public bool IsCharging { get; private set; }
    public bool IsActive { get; private set; }
    //
    public float CooldownTimer;
    public float ComboTimer;
    public float ChargeTimer;
    public float CastTimer;
    public float DurationTimer;
    //
    public int CurrentCombo;
    public int CurrentCharge;
    public int CurrentStack;
    //
    public Vector2 AimPos;
    public Vector2 AimDir;
    public int? ForcedCombo;
    //
    public bool HasCombo => Data.MaxCombo > 1;
    public bool HasCharge => Data.ChargeLimit > 0;
    public bool HasCastTime => Data.CastTime > 0;
    public bool HasDuration => Data.Duration > 0;
    public bool IsAim => Data.MaxRange > 0;
    public bool IsCooldown => CooldownTimer > 0;
    public bool CanCast => !IsCooldown && !IsCasting && !IsActive;
    //
    public event System.Action<AbilityContext> OnTrigger;
    public AbilityContext Create(AbilityPhase phase) {
        return new AbilityContext {
            Caster = Owner,
            Ability = this,
            Combo = CurrentCombo,
            Charge = CurrentCharge,
            AimDir = AimDir,
            AimPos = AimPos,
            Phase = phase
        };
    }
    void Trigger(AbilityPhase phase)
    {
        var ctx = Create(phase);
        ExecutePhase(phase, ctx);
        OnTrigger?.Invoke(ctx);
        //
    }
    void ExecutePhase(AbilityPhase phase, AbilityContext ctx) {
        foreach (var action in Data.Actions) {
            if (action.Phase != phase) continue;
            foreach (var effect in action.Effects) {
                effect.Apply(ctx);
            }
        }
    }
    //
    public bool Press() {
        if (!CanCast) return false;
        Holding = true;
        if (ComboTimer <= 0) {
            CurrentCombo = 0;
            ForcedCombo = null;
        }
        Trigger(AbilityPhase.BeginCast);
        if (HasCharge) {
            StartCharge();
        }
        else if (!IsAim) {
            Release();
        }
        return true;
    }
    public void ReleasePress() {
        Holding = false;
        if (IsCharging || IsAim) {
            Release();
        }
    }
    void StartCharge()
    {
        IsCharging = true;
        ChargeTimer = 0;
    }
    void StopCharge()
    {
        IsCharging = false;
        CurrentCharge = Mathf.Clamp(Mathf.FloorToInt(ChargeTimer), 0, Data.ChargeMax);
    }
    void Release()
    {
        if (IsCooldown) return;
        if (IsCharging) StopCharge();
        Trigger(AbilityPhase.Release);
        if (HasCastTime)
        {
            IsCasting = true;
            CastTimer = Data.CastTime;
        }
         else
        {
            StartActive();
        }
    }
    void StartActive()
    {
        IsCasting = false;
        Trigger(AbilityPhase.CastStart);
        if (HasDuration)
        {
            IsActive = true;
            DurationTimer = Data.Duration;
            Trigger(AbilityPhase.ActiveStart);
        } else { End(); }
    }
    void End()
    {
        IsActive = false;
        Trigger(AbilityPhase.ActiveEnd);
        StartCooldown();
        ResetAIM();
        if (Data.AutoCombo && Holding)
        {
            bool HasNext = Data.LoopCombo || CurrentCombo < Data.MaxCombo - 1;
            if (HasNext)
            {
                StartCombo();
                Release();
            }   
        }
    }
    public void StartCooldown() => CooldownTimer = Data.Cooldown;
    public void StartCombo(int combo = -1)
    {
        if (combo >= 0)
        {
            CurrentCombo = Mathf.Clamp(combo, 0, Data.MaxCombo -1);
        }
        else
        {
            if (CurrentCombo < Data.MaxCombo - 1)
                CurrentCombo++;
            else if (Data.LoopCombo)
                CurrentCombo = 0;
        }
        ComboTimer = Data.ComboWindow;
    }
    public void UpdateAIM(Vector2 value)
    {
        AimPos = value;
        if (value.sqrMagnitude > 0)
            AimDir = value.normalized;
    }
    public void ResetAIM()
    {
        AimPos = Vector2.zero;
        AimDir = Vector2.zero;
    }
    public void Update(float dt)
    {
        NotifyTime.Update(ref CooldownTimer, dt);
        if (NotifyTime.Update(ref ComboTimer, dt))
        {
            CurrentCombo = 0;
        }
        if (IsCharging)
        {
            ChargeTimer += dt;
            ChargeTimer = Mathf.Min(ChargeTimer, Data.ChargeLimit);
            Trigger(AbilityPhase.Charge);
        }
        if (IsCasting)
        {
            CastTimer -= dt;
            if (CastTimer <= 0)
            {
                Trigger(AbilityPhase.CastEnd);
                StartActive();
            }
        }
        if (IsActive)
        {
            DurationTimer -= dt;
            Trigger(AbilityPhase.ActiveUpdate);
            if (DurationTimer <= 0)
            {
                End();
            }   
        }
    }
    public void Cancel()
    {
        Holding = false;
        IsCasting = false;
        IsCharging = false;
        IsActive = false;
        //
        ChargeTimer = 0;
        CastTimer = 0;
        DurationTimer = 0;
        //
        ResetAIM();
        Trigger(AbilityPhase.Cancel);
    }
}

