using UnityEngine;
// sementara sebagian tanpa context agar bisa dipakai di standalone
public interface ITargetable
{
    bool IsPlayer { get; }
    Vector2 Position { get; }
}

// Object version 
public interface IDamageable
{
    bool IsPlayer { get; }
    void ApplyDamage(float damage);
}
public interface IHealable
{
    bool IsPlayer { get; }
    void ApplyHeal(float heal);
}
public interface IBuffable
{
    bool IsPlayer { get; }
    void ApplyBuff(BuffContext ctx);
}
public interface IControllable
{
    void ApplyControll();
}
public interface IMobilizable
{
    void ApplyMobilize();
}
// Container version
public interface IStatHolder
{
    Char_Stats Stats { get; }
}
public interface IBuffHolder
{
    Char_Buff Buffs { get; }
}
public interface IAbilityContainer
{
    Char_Ability Ability { get; }
}
public interface IEffectReceiver
{
    void ApplyEffect(EffectContext ctx);
}
public class EffectContext { }
public interface IMoveable
{
    void Move(Vector2 dir);
}
public interface IKnockbackable
{
    void AddForce(Vector2 force);
}