// SPDX-FileCopyrightText: 2022 CommieFlowers <rasmus.cedergren@hotmail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2022 rolfero <45628623+rolfero@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Doru991 <75124791+Doru991@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Errant <35878406+errant@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 PixelTK <85175107+PixelTheKermit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Vordenburg <114301317+Vordenburg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Callmore <22885888+Callmore@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Dakamakat <52600490+dakamakat@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 John Space <bigdumb421@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 corresp0nd <46357632+corresp0nd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.IdentityManagement;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed partial class StaminaSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly MetaDataSystem _metadata = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffect = default!; // goob edit
    [Dependency] private readonly SharedStutteringSystem _stutter = default!; // goob edit
    [Dependency] private readonly SharedJitteringSystem _jitter = default!; // goob edit
    [Dependency] private readonly ClothingModifyStunTimeSystem _modify = default!; // goob edit
    [Dependency] private readonly IConfigurationManager _config = default!;

    /// <summary>
    /// How much of a buffer is there between the stun duration and when stuns can be re-applied.
    /// </summary>
    private static readonly TimeSpan StamCritBufferTime = TimeSpan.FromSeconds(3f);

    public float UniversalStaminaDamageModifier { get; private set; } = 1f;

    public override void Initialize()
    {
        base.Initialize();

        InitializeModifier();
        InitializeResistance();

        SubscribeLocalEvent<StaminaComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<StaminaComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<StaminaComponent, AfterAutoHandleStateEvent>(OnStamHandleState);
        SubscribeLocalEvent<StaminaComponent, DisarmedEvent>(OnDisarmed);
        SubscribeLocalEvent<StaminaComponent, RejuvenateEvent>(OnRejuvenate);

        SubscribeLocalEvent<StaminaDamageOnEmbedComponent, EmbedEvent>(OnProjectileEmbed);

        SubscribeLocalEvent<StaminaDamageOnCollideComponent, ProjectileHitEvent>(OnProjectileHit);
        SubscribeLocalEvent<StaminaDamageOnCollideComponent, ThrowDoHitEvent>(OnThrowHit);

        SubscribeLocalEvent<StaminaDamageOnHitComponent, MeleeHitEvent>(OnMeleeHit);

        Subs.CVar(_config, CCVars.PlaytestStaminaDamageModifier, value => UniversalStaminaDamageModifier = value, true);
    }

    private void OnStamHandleState(EntityUid uid, StaminaComponent component, ref AfterAutoHandleStateEvent args)
    {
        if (component.Critical)
            EnterStamCrit(uid, component);
        else
        {
            if (component.StaminaDamage > 0f)
                EnsureComp<ActiveStaminaComponent>(uid);

            ExitStamCrit(uid, component);
        }
    }

    private void OnShutdown(EntityUid uid, StaminaComponent component, ComponentShutdown args)
    {
        if (MetaData(uid).EntityLifeStage < EntityLifeStage.Terminating)
        {
            RemCompDeferred<ActiveStaminaComponent>(uid);
        }
        _alerts.ClearAlert(uid, component.StaminaAlert);
    }

    private void OnStartup(EntityUid uid, StaminaComponent component, ComponentStartup args)
    {
        SetStaminaAlert(uid, component);
    }

    [PublicAPI]
    public float GetStaminaDamage(EntityUid uid, StaminaComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0f;

        var curTime = _timing.CurTime;
        var pauseTime = _metadata.GetPauseTime(uid);
        return MathF.Max(0f, component.StaminaDamage - MathF.Max(0f, (float) (curTime - (component.NextUpdate + pauseTime)).TotalSeconds * component.Decay));
    }

    private void OnRejuvenate(EntityUid uid, StaminaComponent component, RejuvenateEvent args)
    {
        if (component.StaminaDamage >= component.CritThreshold)
        {
            ExitStamCrit(uid, component);
        }

        component.StaminaDamage = 0;
        RemComp<ActiveStaminaComponent>(uid);
        SetStaminaAlert(uid, component);
        Dirty(uid, component);
    }

    private void OnDisarmed(EntityUid uid, StaminaComponent component, DisarmedEvent args)
    {
        if (args.Handled)
            return;

        if (component.Critical)
            return;

        var damage = args.PushProbability * component.CritThreshold;
        TakeStaminaDamage(uid, damage, component, source: args.Source);

        args.PopupPrefix = "disarm-action-shove-";
        args.IsStunned = component.Critical;

        args.Handled = true;
    }

    // goobstation - stun resistance. try not to modify this method at all
    private void OnMeleeHit(EntityUid uid, StaminaDamageOnHitComponent component, MeleeHitEvent args)
    {
        if (!args.IsHit ||
            !args.HitEntities.Any() ||
            component.Damage <= 0f)
        {
            return;
        }

        var ev = new StaminaDamageOnHitAttemptEvent();
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        var stamQuery = GetEntityQuery<StaminaComponent>();
        var toHit = new List<(EntityUid Entity, StaminaComponent Component)>();

        // Split stamina damage between all eligible targets.
        foreach (var ent in args.HitEntities)
        {
            if (!stamQuery.TryGetComponent(ent, out var stam))
                continue;

            toHit.Add((ent, stam));
        }

        // goobstation
        foreach (var (ent, comp) in toHit)
        {
            var hitEvent = new TakeStaminaDamageEvent((ent, comp));
            // raise event for each entity hit
            RaiseLocalEvent(ent, hitEvent);

            if (hitEvent.Handled)
                return;

            var damage = component.Damage;

            damage *= hitEvent.Multiplier;

            damage += hitEvent.FlatModifier;

            TakeStaminaDamage(ent, damage / toHit.Count, comp, source: args.User, with: args.Weapon, sound: component.Sound);
        }
    }

    private void OnProjectileHit(EntityUid uid, StaminaDamageOnCollideComponent component, ref ProjectileHitEvent args)
    {
        OnCollide(uid, component, args.Target);
    }

    private void OnProjectileEmbed(EntityUid uid, StaminaDamageOnEmbedComponent component, ref EmbedEvent args)
    {
        if (!TryComp<StaminaComponent>(args.Embedded, out var stamina))
            return;

        TakeStaminaDamage(args.Embedded, component.Damage, stamina, source: uid);
    }

    private void OnThrowHit(EntityUid uid, StaminaDamageOnCollideComponent component, ThrowDoHitEvent args)
    {
        OnCollide(uid, component, args.Target);
    }

    private void OnCollide(EntityUid uid, StaminaDamageOnCollideComponent component, EntityUid target)
    {
        // you can't inflict stamina damage on things with no stamina component
        // this prevents stun batons from using up charges when throwing it at lockers or lights
        if (!TryComp<StaminaComponent>(target, out var stamComp))
            return;

        var ev = new StaminaDamageOnHitAttemptEvent();
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        // goobstation
        var hitEvent = new TakeStaminaDamageEvent((target, stamComp));
        RaiseLocalEvent(target, hitEvent);

        if (hitEvent.Handled)
            return;

        var damage = component.Damage;

        damage *= hitEvent.Multiplier;

        damage += hitEvent.FlatModifier;

        TakeStaminaDamage(target, damage, source: uid, sound: component.Sound);
    }

    private void SetStaminaAlert(EntityUid uid, StaminaComponent? component = null)
    {
        if (!Resolve(uid, ref component, false) || component.Deleted)
            return;

        var severity = ContentHelpers.RoundToLevels(MathF.Max(0f, component.CritThreshold - component.StaminaDamage), component.CritThreshold, 7);
        _alerts.ShowAlert(uid, component.StaminaAlert, (short) severity);
    }

    /// <summary>
    /// Tries to take stamina damage without raising the entity over the crit threshold.
    /// </summary>
    public bool TryTakeStamina(EntityUid uid, float value, StaminaComponent? component = null, EntityUid? source = null, EntityUid? with = null)
    {
        // Something that has no Stamina component automatically passes stamina checks
        if (!Resolve(uid, ref component, false))
            return true;

        var oldStam = component.StaminaDamage;

        if (oldStam + value > component.CritThreshold || component.Critical)
            return false;

        TakeStaminaDamage(uid, value, component, source, with, visual: false);
        return true;
    }

    public void TakeStaminaDamage(EntityUid uid, float value, StaminaComponent? component = null,
        EntityUid? source = null, EntityUid? with = null, bool visual = true, SoundSpecifier? sound = null, bool ignoreResist = false)
    {
        if (!Resolve(uid, ref component, false))
            return;

        var ev = new BeforeStaminaDamageEvent(value);
        RaiseLocalEvent(uid, ref ev);
        if (ev.Cancelled)
            return;

        // Allow stamina resistance to be applied.
        if (!ignoreResist)
        {
            value = ev.Value;
        }

        value = UniversalStaminaDamageModifier * value;

        // Have we already reached the point of max stamina damage?
        if (component.Critical)
            return;

        var oldDamage = component.StaminaDamage;
        component.StaminaDamage = MathF.Max(0f, component.StaminaDamage + value);

        // Reset the decay cooldown upon taking damage.
        if (oldDamage < component.StaminaDamage)
        {
            var nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(component.Cooldown);

            if (component.NextUpdate < nextUpdate)
                component.NextUpdate = nextUpdate;
        }

        var slowdownThreshold = component.CritThreshold / 2f;

        // If we go above n% then apply slowdown
        if (oldDamage < slowdownThreshold &&
            component.StaminaDamage > slowdownThreshold)
        {
            _stunSystem.TrySlowdown(uid, TimeSpan.FromSeconds(3), true, 0.8f, 0.8f);
        }

        SetStaminaAlert(uid, component);

        if (!component.Critical && component.StaminaDamage >= component.CritThreshold && value > 0) // goob edit
            EnterStamCrit(uid, component);
        else if (component.StaminaDamage < component.CritThreshold)
            ExitStamCrit(uid, component);

        EnsureComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);

        if (value <= 0)
            return;
        if (source != null)
        {
            _adminLogger.Add(LogType.Stamina, $"{ToPrettyString(source.Value):user} caused {value} stamina damage to {ToPrettyString(uid):target}{(with != null ? $" using {ToPrettyString(with.Value):using}" : "")}");
        }
        else
        {
            _adminLogger.Add(LogType.Stamina, $"{ToPrettyString(uid):target} took {value} stamina damage");
        }

        if (visual)
        {
            _color.RaiseEffect(Color.Aqua, new List<EntityUid>() { uid }, Filter.Pvs(uid, entityManager: EntityManager));
        }

        if (_net.IsServer)
        {
            _audio.PlayPvs(sound, uid);
        }
    }

    public void ToggleStaminaDrain(EntityUid target, float drainRate, bool enabled, bool modifiesSpeed, EntityUid? source = null)
    {
        if (!TryComp<StaminaComponent>(target, out var stamina))
            return;

        // If theres no source, we assume its the target that caused the drain.
        var actualSource = source ?? target;

        if (enabled)
        {
            stamina.ActiveDrains[actualSource] = (drainRate, modifiesSpeed);
            EnsureComp<ActiveStaminaComponent>(target);
        }
        else
            stamina.ActiveDrains.Remove(actualSource);

        Dirty(target, stamina);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (!_timing.IsFirstTimePredicted)
            return;

        var stamQuery = GetEntityQuery<StaminaComponent>();
        var query = EntityQueryEnumerator<ActiveStaminaComponent>();
        var curTime = _timing.CurTime;
        while (query.MoveNext(out var uid, out _))
        {
            // Just in case we have active but not stamina we'll check and account for it.
            if (!stamQuery.TryGetComponent(uid, out var comp) ||
                comp.StaminaDamage <= 0f && !comp.Critical && comp.ActiveDrains.Count == 0)
            {
                RemComp<ActiveStaminaComponent>(uid);
                continue;
            }
            if (comp.ActiveDrains.Count > 0)
                foreach (var (source, (drainRate, modifiesSpeed)) in comp.ActiveDrains)
                    TakeStaminaDamage(uid,
                    drainRate * frameTime,
                    comp,
                    source: source,
                    visual: false);
            // Shouldn't need to consider paused time as we're only iterating non-paused stamina components.
            var nextUpdate = comp.NextUpdate;

            if (nextUpdate > curTime)
                continue;

            // We were in crit so come out of it and continue.
            if (comp.Critical)
            {
                ExitStamCrit(uid, comp);
                continue;
            }

            comp.NextUpdate += TimeSpan.FromSeconds(1f);
            // If theres no active drains, recover stamina.
            if (comp.ActiveDrains.Count == 0)
                TakeStaminaDamage(uid, -comp.Decay, comp);

            Dirty(uid, comp);
        }
    }

    private void EnterStamCrit(EntityUid uid, StaminaComponent? component = null, bool hardStun = false)
    {
        if (!Resolve(uid, ref component) || !hardStun && component.Critical)
        {
            return;
        }

        // To make the difference between a stun and a stamcrit clear
        // TODO: Mask?

        component.Critical = true;
        component.StaminaDamage = component.CritThreshold;

        _stunSystem.TryParalyze(uid, component.StunTime, true);

        component.NextUpdate = _timing.CurTime + component.StunTime * _modify.GetModifier(uid) + StamCritBufferTime;

        EnsureComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);
        _adminLogger.Add(LogType.Stamina, LogImpact.Medium, $"{ToPrettyString(uid):user} entered stamina crit");
    }

    private void ExitStamCrit(EntityUid uid, StaminaComponent? component = null)
    {
        if (!Resolve(uid, ref component) ||
            !component.Critical)
        {
            return;
        }

        component.Critical = false;
        component.StaminaDamage = 0f;
        component.NextUpdate = _timing.CurTime;
        SetStaminaAlert(uid, component);
        RemComp<ActiveStaminaComponent>(uid);
        Dirty(uid, component);
        _adminLogger.Add(LogType.Stamina, LogImpact.Low, $"{ToPrettyString(uid):user} recovered from stamina crit");
    }
}
