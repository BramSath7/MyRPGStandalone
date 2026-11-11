using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Ability {
    [Header("Identity")] // Default
    public string Name = "";
    [TextArea] public string Desc = "";
    [Header("Modifier")]
    public float Cooldown = 0f;
    public float CastTime = 0f;
    public float Duration = 0f;
    [Header("Scale")]
    public float Scale = 1f;
    [Header("AIM")]
    public float minRange = 0f;
    public float maxRange = 0f;
    [Header("Combo")]
    public int MaxCombo = 1;
    public float ComboWindow = 1.5f;
    public bool AutoCombo = false;
    public bool LoopCombo = false;
    public int? ForcedCombo;
    [Header("Charge")]
    public float ChargeLimit = 0f;
    public int ChargeMax = 1;
    [Header("Stack")]
    public bool Stack;
    public int StackLimit = 1;
    public List<Sprite> Icons = new();
    [Header("Runtime")]
    public List<Hitbox_Ability> Hitboxes = new();
    public Level Level = new();
    //
    public bool HasCombo => MaxCombo > 1;
    public bool HasCharge => ChargeLimit > 0;
    public bool HasCastTime => CastTime > 0;
    public bool HasDuration => Duration > 0;
    public bool IsAim => maxRange > 0;
    //
    public bool Holding { get; private set; }
    public bool IsCasting { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCharging { get; private set; }
    public bool IsCooldown => CooldownTimer > 0;
    public bool CanCast => !IsCasting && !IsActive && !IsCooldown;
    // Timer
    public float CooldownTimer;
    public float ComboTimer;
    public float ChargeTimer { get; private set; }
    public float CastTimer { get; private set; }
    public float DurationTimer { get; private set; }
    // Currents 
    public int CurrentCombo { get; private set; }
    public int CurrentCharge { get; private set; }
    public int StackCurrent { get; private set; }
    int LastChargeLevel = -1;
    // 
    public Vector2 AimPos { get; private set; }
    public Vector2 AimDir { get; private set; }
    public Sprite CurrentIcon
        => Icons.Count == 0 ? null : Icons[Mathf.Clamp(CurrentCombo, 0, Icons.Count - 1)];
    // Cast 
    public event System.Action<Ability> OnBeginCast;
    public event System.Action<Ability> OnRelease;  // start combo 
    // Charge
    public event System.Action<Ability> OnCharge;
    public event System.Action<int> OnChargeLevelChanged;
    // Cast Time
    public event System.Action<Ability> OnCastStart;
    public event System.Action<Ability> OnCastEnd;
    // Active
    public event System.Action<Ability> OnActiveStart;
    public event System.Action<Ability> OnActiveUpdate;
    public event System.Action<Ability> OnActiveEnd;
    // Combo
    public event System.Action<int> OnComboChanged;
    public event System.Action<int> OnComboStart;
    public event System.Action<int> OnComboEnd;
    // Stack 
    public event System.Action<int> OnStackChanged;
    // Cooldown
    public event System.Action<Ability> OnCooldownStart;
    public event System.Action<Ability> OnCooldownEnd;
    private bool cooldownNotified;
    // 
    public event System.Action<Vector2> OnAimChanged;
    // Cancel
    public event System.Action<Ability> OnCancel;
    // Funx
    public bool Press() {
        Holding = true;
        if (!CanCast) return false;
        if (ComboTimer <= 0) {
            CurrentCombo = 0;
            ForcedCombo = null;
            OnComboStart?.Invoke(CurrentCombo);
        }
        OnBeginCast?.Invoke(this);
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
        if (IsCharging||IsAim) Release();
    }
    private void StartCharge() {
        IsCharging = true;
        ChargeTimer = 0;
        LastChargeLevel = -1;
    }
    private void StopCharge() {
        IsCharging = false;
        CurrentCharge = Mathf.Clamp(Mathf.FloorToInt(ChargeTimer), 0, ChargeMax);
    }
    private void Release() {
        if (IsCooldown) return;
        if (IsCharging) StopCharge();
        OnRelease?.Invoke(this);
        if (HasCastTime) {
            IsCasting = true;
            CastTimer = CastTime;
            OnCastStart?.Invoke(this);
        }
        else {
            StartActive();
        }
    }
    private void StartActive() {
        IsCasting = false;
        OnActiveStart?.Invoke(this);
        if (HasDuration) {
            IsActive = true;
            DurationTimer = Duration;
        }
        else {
            OnActiveEnd?.Invoke(this);
            End();
        }
    }
    private void End() {
        IsActive = false;
        ResetAIM();
        StartCooldown();
        if (AutoCombo && Holding) {
            bool hasNext = LoopCombo || CurrentCombo < MaxCombo - 1;
            if (hasNext) {
                StartCombo();
                Release();
                return;
            }
        }
    }
    //
    public void StartCombo(int combo = -1) {
        if (combo >= 0) {
            CurrentCombo = Mathf.Clamp(combo, 0, MaxCombo - 1);
        }
        else {
            if (CurrentCombo < MaxCombo - 1) {
                CurrentCombo++;
                OnComboChanged?.Invoke(CurrentCombo);
            }
            else if (LoopCombo) {
                CurrentCombo = 0;
                OnComboEnd?.Invoke(CurrentCombo);
            }
        }
        ComboTimer = ComboWindow;
    }
    public void SetForcedCombo(int combo) {
        ForcedCombo = Mathf.Clamp(combo, 0, MaxCombo - 1);
    }
    public int GetCombo() {
        if (ForcedCombo.HasValue) {
            int combo = ForcedCombo.Value;
            ForcedCombo = null;
            return combo;
        }
        return CurrentCombo;
    }
    //
    public void AddStack(int amount) {
        int before = StackCurrent;
        StackCurrent = Mathf.Min(StackCurrent + amount, StackLimit);
        if (before != StackCurrent) {
            OnStackChanged?.Invoke(StackCurrent);
        }
    }
    public void ConsumeStack(int amount) {
        int before = StackCurrent;
        StackCurrent = Mathf.Max(StackCurrent - amount, 0);
        if (before != StackCurrent) {
            OnStackChanged?.Invoke(StackCurrent);
        }
    }
    //
    public void StartCooldown() {
        CooldownTimer = Cooldown;
        OnCooldownStart?.Invoke(this);
    }
    //
    public void UpdateAIM(Vector2 newInput) {
        AimPos = newInput;
        if (newInput.sqrMagnitude > 0) {
            AimDir = AimPos.normalized;
        }
        OnAimChanged?.Invoke(AimPos);
    }
    public void ResetAIM() {
        AimPos = Vector2.zero;
        AimDir = Vector2.zero;
    }
    //
    public void Update(float dt) {
        if (NotifyTime.Update(ref CooldownTimer, dt)){
            OnCooldownEnd?.Invoke(this);
        }
        if (NotifyTime.Update(ref ComboTimer, dt)) {
            if (CurrentCombo > 0) {
                CurrentCombo = 0;
                OnComboEnd?.Invoke(0);
            }
        }
        if (IsCharging) {
            ChargeTimer += dt;
            ChargeTimer = Mathf.Min(ChargeTimer, ChargeLimit);
            OnCharge?.Invoke(this);
            int level = Mathf.Clamp(Mathf.FloorToInt(ChargeTimer), 0, ChargeMax);
            if (level != LastChargeLevel) {
                LastChargeLevel = level;
                OnChargeLevelChanged?.Invoke(level);
            }
        }
        if (IsCasting) {
            CastTimer -= dt;
            if (CastTimer <= 0) {
                OnCastEnd?.Invoke(this);
                StartActive();
            }
        }
        if (IsActive) {
            DurationTimer -= dt;
            OnActiveUpdate?.Invoke(this);
            if (DurationTimer <= 0) {
                OnActiveEnd?.Invoke(this);
                End();
            }
        }
    }
    public void Cancel() {
        Holding = false;
        //
        IsCasting = false;
        IsCharging = false;
        IsActive = false;
        //
        CastTimer = 0;
        ChargeTimer = 0;
        DurationTimer = 0;
        //
        ResetAIM();
        //
        OnCancel?.Invoke(this);
    }
}
//

public class ComboStep {
    public string Animation;
    public Sprite Icon;
    public float Scale = 1f;
}


//
[System.Serializable]
public class HitboxGroup
{
    public string ID;
    public List<Ability_Phase> Phase = new();
}
[System.Serializable]
public class HitStep
{
    public float Ratio;
}
[System.Serializable]
public class Ability_Phase {
    public string Animation;
    public float Scale;
    public Sprite Icon;
    public List<Hitbox_Ability> Hitboxes = new();
}
