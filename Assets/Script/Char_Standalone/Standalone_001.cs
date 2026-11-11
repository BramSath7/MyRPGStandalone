using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Adventurer : Unit {
    public Adventurer() {
        Name = "LoserLord";
        Health = new GamePoint(500f);
        Energy = new GamePoint(100f);
        Attack = new GamePoint(20f);
        //
        Basic = new Ability {
            Name = "Combine",
            Cooldown = 0.5f,
            Level = new Level {
                CurrentLevel = 1,
                CurrentExp = 0,
                MaxLevel = 10
            },
            Icons = new List<Sprite> {
                Resources.Load<Sprite>("Icons/Combo_1")
            },
            MaxCombo = 1,
        };
        Skill = new Ability {
            Name = "Power Slash",
            Cooldown = 5f,
            ChargeLimit = 2f,
            ChargeMax = 3,
            Scale = 2f,
            maxRange = 4f
        };
        Spell = new Ability {
            Name = "Light Beam",
            Cooldown = 8f,
            CastTime = 1f,
            Duration = 5f,
            maxRange = 10f
        };
        Ultimate = new Ability {
            Name = "Whirlwind",
            Icons = new List<Sprite> {
                Resources.Load<Sprite>("Icons/Head_Knight")
            } ,
            Cooldown = 15f,
            Duration = 4f,
            Scale = 3f
        };
    }
    public override void OnCombatStart() {
        Basic.OnBeginCast += BasicBegin;
        Basic.OnRelease += BasicRelease;
        Basic.Hitboxes = LoadedUnit.abilityCol[0].Hitboxes;
        foreach (var h in Basic.Hitboxes) {
            h.OnHit += OnBasic;
        }
        //
        Skill.OnCharge += SkillCharge;
        Skill.OnRelease += SkillRelease;
        base.OnCombatStart();
    }
    public override void OnCombatEnd() {
        Basic.OnBeginCast -= BasicBegin;
        Basic.OnRelease -= BasicRelease;
        base.OnCombatEnd();
    }
    //
    private void BasicBegin(Ability a) {
        int combo = a.GetCombo();
        switch (combo) {
            case 1:
                LoadedUnit.HandleAnim(a.AimDir,"Slash");
                break;
        }
    }
    private void BasicRelease(Ability a) {
        a.StartCombo();
        Debug.Log("Basic Cast");
    }
    private void OnBasic(Collider2D other) {
        if (!other.TryGetComponent<IDamageable>(out var target)) return;
        if (target.IsPlayer == IsPlayer) return;
        float damage = Attack.CurrentTotal * Basic.Scale;
        target.ApplyDamage(damage);
    }
    //
    private void SkillCharge(Ability a) {
        // Play Effect 

    }
    private void SkillRelease(Ability a) {
        // play anim
        Basic.SetForcedCombo(2);
        float bonus = 1f + (a.CurrentCharge * 0.5f);
        a.Scale = 2f * bonus;
        Debug.Log($"Released Lv {a.CurrentCharge}");
    }
    //
    private void SpellRelease(Ability a) {
        // play anim
    }
    private void SpellCasting(Ability a) {
        // facedir 
        Debug.Log($"Casting: {a.CastTimer: 0.0}");
    }
    private void SpellActive(Ability a) {
        // facedir 
        // BeamEffect.spawn("", transform.pos)
        // BeamEffect.transform.pos = transform.pos
    }
    private void BeamEnd(Ability a) {
        // beameffect.destroy
    }
    //
    private void UltimateRelease()
    {
        // playanim
    }
    private void UltimateActive()
    {

    }
    private void UltimateEnd()
    {

    }
}
// Instant Slash
// Coodown 
// OnRelease  
// OnActiveStart 

// Fire Ball 
// Cooldown     |   CastTime
// CastStart    |   CastEnd         |   Cooldown

// Charge Shot
// ChargeLimit  |   ChargeMax
// Charge       |   ChargeLevel
// Release 

// Combo 
// MaxCombo     |   ComboWindow
// ComboStart   |   ComboChanged    |   ComboEnd
// Release 

// Aura 
// Duration 
// ActiveStart  |   ActiveUpdate    |   ActiveEnd

// Dash 
// Cooldown 
// Release 
// ActiveStart  |   ActiveEnd

// Summon 
// OnRelease