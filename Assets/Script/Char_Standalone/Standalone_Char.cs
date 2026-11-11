using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit {
    public string Name;
    public Level Level = new();
    [Header("Stats")]
    public GamePoint Health;
    public GamePoint Energy;
    public GamePoint Attack;
    //
    public Unit_Buff Buff = new();
    //
    [Header("Abilities")]
    public Ability Basic;
    public Ability Skill;
    public Ability Spell;
    public Ability Ultimate;
    public Ability OverrideBasic;
    public Ability OverrideSkill;
    public Ability OverrideSpell;
    //
    public bool Mode;
    public Ability CurrentBasic => Mode ? OverrideBasic : Basic;
    public Ability CurrentSkill => Mode ? OverrideSkill : Skill;
    public Ability CurrentSpell => Mode ? OverrideSpell : Spell;
    //
    public bool IsInitialized;
    public bool IsPlayer;
    public bool IsAlive => !Health.IsEmpty;
    public bool IsAI;
    public bool CanMove => 
        (CurrentState == CharState.Idle || 
        CurrentState == CharState.Moving)
        && !IsImobilized;
    public bool IsImobilized = false;
    //
    public Object_Character LoadedUnit;
    public Vector2 CurrentPos;
    public Vector2 CurrentDir;
    public CharState CurrentState = CharState.Idle;
    //
    public virtual void OnCombatStart() { IsInitialized = true; }
    public virtual void OnCombatEnd() { IsInitialized = false; }
    public void Update(float dt) {
        if (!IsInitialized) return;
        Health.Tick(dt);
        Energy.Tick(dt);
        Attack.Tick(dt);
        //
        Buff.Update(dt);
        //
        Basic.Update(dt);
        Skill.Update(dt); 
        Spell.Update(dt); 
        Ultimate.Update(dt);
    }
}
//
public enum CharState { 
    Idle, Moving, Died, 
    Casting
}
public enum ControllState {
    None,
}
public enum AbilityState {
    None,
    Casting,
    Charging,
    Active,
    Cooldown
}
public enum Rarity {
    Common, Rare, Unique, Epic, Legend, Myth
}
public enum StatType {
    Health, 
    Energy, Mana, QI,
    AttackPower, AttackIntensity, AttackCrit, AttackCritDmg, AttackForce,
    Defense, Resistance, Weight,
    FireDmg, IceDmg, EarthDmg, LighningDmg,
}