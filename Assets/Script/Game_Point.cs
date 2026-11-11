using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class GamePoint {
    // Base
    public float BaseFlat;
    public float BaseEfficient = 1f;
    // Runtime
    public float Minimal;
    public float Current;
    public float Corrupt;
    //
    private readonly List<PointModifier> Modifiers = new();
    //
    private float cachedRawTotal;
    private float cachedCurrentTotal;
    private bool dirty = true;
    private bool wasEmpty;
    public bool IsLocked { get; set; }
    //
    public float RawTotal
    {
        get
        {
            if (dirty) RecalcTotals();
            return cachedRawTotal;
        }
    }
    public float CurrentTotal
    {
        get
        {
            if (dirty) RecalcTotals();
            return cachedCurrentTotal;
        }
    }
    //
    public float Ratio =>
        CurrentTotal <= 0f ? 0f : Mathf.Clamp01(Current / CurrentTotal);
    public float Percent =>
        Ratio * 100f;
    public float Missing => CurrentTotal - Current;
    public bool IsEmpty =>
        Current <= Minimal;
    public bool IsFull => Mathf.Approximately(Current, CurrentTotal);
    //
    public event System.Action OnEmpty;
    public event System.Action<float> OnTick;
    public event System.Action<float, float> OnValue;
    public event System.Action<float, float> OnCapacity;
    //
    public GamePoint(float basePoint, float min = 0f)
    {
        BaseFlat = basePoint;
        Minimal = Mathf.Max(0f, min);
        Refresh();
        Current = CurrentTotal;
    }
    float totalFlat => Modifiers.Sum(x => x.Flat);
    float totalPercent => Modifiers.Sum(s => s.Percent);
    private void RecalcTotals() {
        cachedRawTotal = (BaseFlat * BaseEfficient) * (1f + totalPercent) + totalFlat;
        cachedCurrentTotal = Mathf.Max(Minimal, cachedRawTotal - Corrupt);
        dirty = false;
    }
    private void ClampCurrent() {
        Current = Mathf.Clamp(Current, Minimal, CurrentTotal);
    }
    private void ApplyChange(System.Action act) {
        float beforeValue = Current;
        float beforeCapacity = CurrentTotal;
        act?.Invoke();
        dirty = true;
        RecalcTotals();
        ClampCurrent();
        if(!Mathf.Approximately(beforeValue, Current)) {
            OnValue?.Invoke(beforeValue, Current);
        }
        if (!Mathf.Approximately(beforeCapacity, CurrentTotal)) {
            OnCapacity?.Invoke(beforeCapacity, CurrentTotal);
        }
    }
    //
    public void SetBaseFlat(float value)
        => ApplyChange(() => BaseFlat = value);
    public void SetBaseEfficient(float value)
        => ApplyChange(() => BaseEfficient = value);
    public void ModifyCorrupt(float value)
        => ApplyChange(() => Corrupt = Mathf.Max(0f, Corrupt + value));
    //
    public void AddModifier(PointModifier mod)
    {
        ApplyChange(() => Modifiers.Add(mod));
    }
    public void RemoveModifier(PointModifier mod)
    {
        ApplyChange(() => { Modifiers.Remove(mod); });
    }
    public void RemoveModifier(string source)
    {
        ApplyChange(() => {
            Modifiers.RemoveAll(x => x.SourceID == source);
        });
    }
    //
    public float ModifyCurrent(float value) {
        float before = Current;
        ApplyChange(() => { Current += value; });
        return Current - before;
    }
    public float Decay(float value)
        => ModifyCurrent(-Mathf.Abs(value));
    public float Regen(float value)
        => ModifyCurrent(Mathf.Abs(value));
    //
    public void Reset()
    {
        float before = Current;
        dirty = true;
        RecalcTotals();
        Current = CurrentTotal;
        if (!Mathf.Approximately(before, Current)) {
            OnValue?.Invoke(before, Current);
        }
    }
    public void Refresh()
    {
        float beforeCapacity = CurrentTotal;
        dirty = true;
        RecalcTotals();
        ClampCurrent();
        if (!Mathf.Approximately(beforeCapacity, CurrentTotal)) {
            OnCapacity?.Invoke(beforeCapacity, CurrentTotal);
        }
    }
    public void Cleanse() {
        float before = Current;
        Current = Minimal;
        if (!Mathf.Approximately(before, Current)) {
            OnValue?.Invoke(before, Current);
        }
    }
    //
    public virtual void Tick(float dt) {
        OnTick?.Invoke(dt);
        bool emptyNow = IsEmpty;
        if (emptyNow && !wasEmpty) {
            OnEmpty?.Invoke();
        }
        wasEmpty = emptyNow;
    }
    // Obsolete Parts
    public float BonusPercent;
    public float BonusFlat;
    public void ModifyBasePercent(float value)
        => ApplyChange(() => BaseEfficient += value);
    public void ModifyBonusPercent(float value)
        => ApplyChange(() => BonusPercent += value);
    public void ModifyBonusFlat(float value)
        => ApplyChange(() => BonusFlat += value);
    public void ResetBaseEfficient()
    {
        SetBaseEfficient(1f);
    }
}


public class PointModifier {
    public string SourceID;

    public float Flat;
    public float Percent;
}

