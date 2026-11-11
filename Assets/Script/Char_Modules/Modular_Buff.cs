using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Buff")]
public class Buff_Data : ScriptableObject {
    public string ID;
    public string Name;
    public Sprite Icon;
    //
    public float Duration;
    public int MaxStack = 1;
    //
    public bool RefreshDuration = true;
    public bool RefreshStack = true;
    //
    public List<BuffAction> Actions = new();
}
public enum BuffPhase { Apply, Update, Remove }
[System.Serializable]
public class BuffAction {
    public BuffPhase Phase;
    [SerializeReference]
    [SubclassSelector]
    public List<Buff_Effect> Effects = new();
}
[System.Serializable]
public class Buff_Instance {
    public Buff_Data Data;
    [System.NonSerialized] public Char_Runtime Owner;
    //
    public float Timer;
    public int Stack = 1;
    public Buff_Instance(Buff_Data data, Char_Runtime owner) {
        Data = data;
        Owner = owner;
        Timer = data.Duration;
    }
    public void Apply()
    {
        var ctx = CreateContext();
        foreach (var action in Data.Actions) {
            if (action.Phase != BuffPhase.Apply) continue;
            foreach (var eff in action.Effects) {
                eff.OnApply(ctx);
            }
        }
    }
    public void Update(float dt)
    {
        Timer -= dt;
        var ctx = CreateContext();
        foreach (var action in Data.Actions)
        {
            if (action.Phase != BuffPhase.Update) continue;
            foreach (var eff in action.Effects) 
                eff.OnUpdate(ctx, dt);
        }
        if (Timer <= 0) Remove();
    }
    public void Remove()
    {
        var ctx = CreateContext();
        foreach (var action in Data.Actions)
        {
            if (action.Phase != BuffPhase.Remove) continue;
            foreach (var eff in action.Effects)
                eff.OnRemove(ctx);
        }
    }
    BuffContext CreateContext()
    {
        return new BuffContext
        {
            Owner = Owner,
            Buff = this,
            Stack = Stack,
        };
    }
}
[System.Serializable]
public abstract class Buff_Effect {
    public virtual void OnApply(BuffContext ctx) { }
    public virtual void OnRemove(BuffContext ctx) { }
    public virtual void OnUpdate(BuffContext ctx, float dt) { }
}
//
[System.Serializable]
public class RegenEffect: Buff_Effect {
    float Heal;
    public override void OnUpdate(BuffContext ctx, float dt) {
        ctx.Owner.Stats.Health.Regen(Heal);
    }
}
[System.Serializable]
public class AttackEffect: Buff_Effect {
    public float BonusFlat;
    public float BonusPercent;
    PointModifier modifier;
    public override void OnApply(BuffContext ctx) {
        modifier = new PointModifier {
            SourceID = ctx.Buff.Data.ID,
            Flat = BonusFlat,
            Percent = BonusPercent
        };
        ctx.Owner.Stats.Attack.AddModifier(modifier);
    }
    public override void OnRemove(BuffContext ctx) {
        ctx.Owner.Stats.Attack.RemoveModifier(modifier);
    }
}