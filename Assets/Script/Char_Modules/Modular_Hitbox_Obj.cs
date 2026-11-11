using System.Collections.Generic;
using UnityEngine;

public class Modular_Hitbox_Obj : MonoBehaviour
{
    public Hitbox_Data Data { get; private set; }

    private AbilityContext context;

    private readonly HashSet<Char_Runtime> hits = new();
    private readonly Dictionary<Char_Runtime, float> hitTimers = new();

    private float lifeTimer;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (!anim)
            anim = GetComponent<Animator>();
    }

    public void Initialize(
        Hitbox_Data data,
        AbilityContext ctx)
    {
        Data = data;
        context = ctx;

        lifeTimer = data.Duration;

        Vector2 pos =
            ctx.Caster.CurrentPos +
            ctx.Caster.CurrentDir * data.Distance +
            data.Offset;

        transform.position = pos;

        RotateToDirection();

        if (Data.SpawnSFX)
        {
            AudioSource.PlayClipAtPoint(
                Data.SpawnSFX,
                pos);
        }
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        UpdateFollow();

        UpdateMultiHit();

        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateFollow()
    {
        if (!Data.FollowCaster)
            return;

        if (context?.Caster == null)
            return;

        transform.position =
            context.Caster.CurrentPos +
            context.Caster.CurrentDir * Data.Distance +
            Data.Offset;

        RotateToDirection();
    }

    void RotateToDirection()
    {
        Vector2 dir = context.Caster.CurrentDir;

        if (dir.sqrMagnitude <= 0)
            return;

        float angle =
            Mathf.Atan2(
                dir.y,
                dir.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0,
                0,
                angle);
    }

    void UpdateMultiHit()
    {
        if (!Data.MultiHit)
            return;

        List<Char_Runtime> expired = null;

        foreach (var pair in hitTimers)
        {
            if (pair.Value > Time.time)
                continue;

            expired ??= new();
            expired.Add(pair.Key);
        }

        if (expired == null)
            return;

        foreach (var unit in expired)
        {
            hitTimers.Remove(unit);
        }
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        var obj =
            other.GetComponentInParent<Module_Char_Obj>();

        if (obj == null)
            return;

        Char_Runtime target = obj.Runtime;

        if (target == null)
            return;

        if (target == context.Caster)
            return;

        if (!CanHit(target))
            return;

        ApplyHit(target, other);
    }

    bool CanHit(Char_Runtime target)
    {
        if (!Data.MultiHit)
        {
            return hits.Add(target);
        }

        if (hitTimers.TryGetValue(
            target,
            out float nextTime))
        {
            if (nextTime > Time.time)
                return false;
        }

        hitTimers[target] =
            Time.time + Data.HitInterval;

        return true;
    }

    void ApplyHit(
        Char_Runtime target,
        Collider2D collider)
    {
        HitContext hitCtx = new()
        {
            Caster = context.Caster,
            Target = target,

            Ability = context.Ability,

            Phase = AbilityPhase.Hit,

            Combo = context.Combo,
            Charge = context.Charge,

            AimDir = context.AimDir,
            AimPos = context.AimPos,

            collider = collider,
            HitPoint = collider.bounds.center
        };

        foreach (var effect in Data.Effects)
        {
            effect.Apply(hitCtx);
        }
    }
}