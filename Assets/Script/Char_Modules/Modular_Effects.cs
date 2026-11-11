using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AbilityAction {
    public AbilityPhase Phase;

    [SerializeReference]
    [SubclassSelector]
    public List<Ability_Effect> Effects = new();
}
[System.Serializable]
public abstract class Ability_Effect {
    public abstract void Apply(AbilityContext ctx);
}
public enum AbilityPhase {
    BeginCast, Charge, Release, 
    CastStart, CastEnd, 
    ActiveStart, ActiveUpdate, ActiveEnd, 
    Hit, Cancel
}
//
public class AbilityContext {
    public Char_Runtime Caster;
    public Char_Runtime Target;
    public AbilityRuntime Ability;
    public AbilityPhase Phase;
    public int Combo;
    public int Charge;
    public Vector2 AimDir;
    public Vector2 AimPos;
    public Vector2 Pos => Caster.CurrentPos;
    public Module_Char_Obj Character => Caster.LoadedObj;
}
//

public class HitContext: AbilityContext {
    public Collider2D collider;
    public Vector2 HitPoint;
}
public class BuffContext
{
    public Char_Runtime Owner;
    public Buff_Instance Buff;
    public BuffPhase Phase;
    public int Stack;
}
//
[System.Serializable]
public class DamageEffect: Ability_Effect {
    public float Damage;
    public override void Apply(AbilityContext ctx) {
        if (ctx.Target == null) return;
        ctx.Target.Stats.Health.ModifyCurrent(-Damage);
    }
}
[System.Serializable]
public class HealEffect: Ability_Effect {
    public float Amount;
    public override void Apply(AbilityContext ctx)
    {
        ctx.Target.Stats.Health.ModifyCurrent(Amount);
    }
}
[System.Serializable]
public class BuffEffect: Ability_Effect {
    public Buff_Data Buff;
    public override void Apply(AbilityContext ctx) {
        if (ctx.Target == null) return;
        ctx.Target.Buffs.Add(Buff);
    }
}
[System.Serializable]
public class AddMod: Ability_Effect
{
    public string StatID;
    public float Flat;
    public float Percent;
    public override void Apply(AbilityContext ctx)
    {
        if (!ctx.Caster.Stats.Stats.TryGetValue(StatID, out var stats)) return;
        stats.AddModifier(new PointModifier {
            SourceID = ctx.Ability.Data.ID,
            Flat = Flat,
            Percent = Percent
        });
    }
}
//
[System.Serializable]
public class SpawnHitbox: Ability_Effect {
    public Hitbox_Data Hitbox;
    public override void Apply(AbilityContext ctx) {
        Vector2 pos = ctx.Caster.CurrentPos + ctx.Caster.CurrentDir * Hitbox.Distance + Hitbox.Offset;
        var obj = UnityEngine.Object.Instantiate(Hitbox.Prefab, pos, Quaternion.identity);
        obj.GetComponent<Modular_Hitbox_Obj>().Initialize(Hitbox, ctx);
    }
}
public class SpawnProjectile: Ability_Effect
{
    public override void Apply(AbilityContext ctx)
    {
        
    }
}
//

[System.Serializable]
public class PlayAnim: Ability_Effect {
    public string Trigger;
    public override void Apply(AbilityContext ctx)
    {
        ctx.Caster.LoadedObj.anim.SetTrigger(Trigger);
    }
}
[System.Serializable]
public class PlaySFX: Ability_Effect {
    public AudioClip Clip;
    public override void Apply(AbilityContext ctx) {
        if (!Clip) return;
        ctx.Caster.LoadedObj.ability.PlayOneShot(Clip);
    }
}
[System.Serializable]
public class PlayVFX: Ability_Effect
{
    public GameObject Prefab;
    public override void Apply(AbilityContext ctx)
    {
        if (!Prefab) return;
        UnityEngine.Object.Instantiate(Prefab, ctx.Pos, Quaternion.identity);
    }
}
//
[System.Serializable]
public class FaceAim: Ability_Effect
{
    public override void Apply(AbilityContext ctx) {
        ctx.Caster.SetDir(ctx.AimDir.normalized);
    }
}
public class MoveTo: Ability_Effect
{
    public override void Apply(AbilityContext ctx)
    {
        
    }
}
//
[System.Serializable]
public class StackRegen: Ability_Effect
{
    public int Amount = 1;
    public override void Apply(AbilityContext ctx)
    {
        
    }
}
[System.Serializable]
public class StackConsume: Ability_Effect
{
    public int Amount;
    public override void Apply(AbilityContext ctx)
    {
        
    }
}