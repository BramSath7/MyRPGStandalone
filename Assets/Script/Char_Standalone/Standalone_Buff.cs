using System.Collections.Generic;
using UnityEngine;
public class Unit_Buff {
    private readonly List<BuffInst> effects = new();
    public IReadOnlyList<BuffInst> Effects => effects;
    public void Update(float dt) {
        for (int i = effects.Count - 1; i >= 0; i--) {
            var effect = effects[i];
            effect.Update(dt);

            if (effect.IsRemoved) {
                effects.RemoveAt(i);
            }
        }
    }
    public void Add(BuffInst effect) {
        var exist = Get(effect.ID);
        if (exist != null) {
            exist.AddStack();
            return;
        }
        effects.Add(effect);
    }
    public bool Remove(BuffInst effect) {
        if (effect == null) return false;
        effect.Remove();
        return effects.Remove(effect);
    }
    public void Remove(string id) {
        for (int i = effects.Count - 1; i >= 0; i--) {
            if (effects[i].ID == id) {
                effects[i].Remove();
                effects.RemoveAt(i);
            }
        }
    }
    public bool Has(string id) {
        foreach (var effect in effects) {
            if (effect.ID == id) return true;
        }
        return false;
    }
    public BuffInst Get(string id) {
        foreach (var effect in effects) {
            if (effect.ID == id) return effect;
        }
        return null;
    }
    public List<BuffInst> GetAll(string id) {
        List<BuffInst> result = new();

        foreach (var effect in effects) {
            if (effect.ID == id) {
                result.Add(effect);
            }
        }

        return result;
    }
    public void Clear() {
        foreach (var effect in effects) {
            effect.Remove();
        }
        effects.Clear();
    }
}
public class BuffInst {
    public string ID;
    public string[] Tags;
    public Unit Source { get; set; }
    public Unit Owner {  get; set; }
    public StackMode Mode { get; set; } = StackMode.Linear;
    public float Duration { get; set; } = -1;
    public float Interval { get; set; } = 0f;
    public float Cooldown { get; set; } = 0f;
    //
    public int RemovableLevel { get; set; } = 1;
    public int MaxStack { get; set; } = 1;
    public int Charges { get; set; } = 1;
    // 
    public bool OnlyExternalTrigger { get; set; } = false;
    public bool CanTriggerExternal { get; set; } = false;
    public bool DecayStack { get; set; } = false;
    public bool RefreshOnApply { get; set; } = false;
    public bool RemoveOnChargeEmpty { get; set; } = false;
    //
    public float StackPercent { get; set; } = 0f;
    public float BaseValue { get; set; } = 0f;
    public float AppliedValue {
        get {
            switch (Mode) {
                case StackMode.Linear:
                    return BaseValue * (ScaledValue * currentStack);
                case StackMode.Percentage:
                    return BaseValue * ScaledValue * (1 + StackPercent * currentStack);
                case StackMode.Additive:
                    return BaseValue * (ScaledValue + currentStack);
                default: 
                    return BaseValue * ScaledValue;
            }
        }
    }
    public float ChanceTrigger { get; set; } = 0f;
    //
    public bool IsActive { get; private set; } = true;
    public bool IsRemoved { get; private set; } = false;
    public int currentStack { get; private set; } = 0;
    //
    private float durationTimer;
    private float intervalTimer;
    private float cooldownTimer;
    private float ScaledValue = 1f;

    public event System.Action<BuffInst> OnTrigger;
    public event System.Action<BuffInst> OnRemove;
    public event System.Action<BuffInst> OnDisable;
    public event System.Action<BuffInst> OnChargeDepleted;
    public void Initialize() {
        durationTimer = Duration;
        intervalTimer = Interval;
    }
    public void SetActive(bool active) {
        if (IsActive == active) return;
        IsActive = active;
        if (!IsActive) {
            OnDisable?.Invoke(this);
        }
    }
    public void Update(float dt) {
        if(!IsActive) return;
        if (cooldownTimer > 0f) {
            cooldownTimer -= dt;
        }
        if (OnlyExternalTrigger) return;
        if (IsRemoved) return;
        //
        if (Duration < 0f) {
            CheckInterval(dt);
            return;
        }
        //
        durationTimer-= dt;
        CheckInterval(dt);
        //
        if (Duration > 0f && durationTimer <= 0f) {
            if (DecayStack && currentStack > 1) {
                currentStack--;
                durationTimer = Duration;
                intervalTimer = Interval;
            } else {
                Remove();
            }
        }
    }
    public void CheckInterval(float dt) {
        if (Interval > 0f) {
            intervalTimer -= dt;
            if (intervalTimer <= 0f) {
                CheckTrigger(); intervalTimer = Interval;
            }
        }
    }
    public void CheckTrigger(bool ConsumeCharge = false) {
        if (cooldownTimer > 0f) return;
        if (CanTriggerExternal) {
            if (ChanceTrigger > 0f && UnityEngine.Random.Range(0f, 100f) >= ChanceTrigger) {
                return;
            }
        }
        OnTrigger?.Invoke(this);
        if (ConsumeCharge) {
            Charges--;
            if (Charges <= 0 && RemoveOnChargeEmpty) {
                OnChargeDepleted?.Invoke(this);
                Remove();
            }
        }
        cooldownTimer = Cooldown;
        if (durationTimer <= 0f && Duration > 0f) {
            Remove();
        }
    }
    public void AddStack() {
        if (MaxStack > 0 && currentStack < MaxStack) {
            currentStack++;
        }
        if (RefreshOnApply) {
            if (Duration > 0f) { durationTimer = Duration; }
            if (Interval > 0f) { intervalTimer = Interval; }
        }
    }
    public void ApplyScale(float scale) => ScaledValue = scale;
    public void RegenCharge(int add) => Charges += add;
    public void Remove() {
        if (IsRemoved) return;
        IsRemoved = true;
        durationTimer = 0f;
        intervalTimer = 0f;
        OnRemove?.Invoke(this);
    }
}
public enum StackMode { Linear, Percentage, Additive}