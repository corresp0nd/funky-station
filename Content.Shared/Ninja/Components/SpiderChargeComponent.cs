// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Ninja.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Ninja.Components;

/// <summary>
/// Component for the Space Ninja's unique Spider Charge.
/// Only this component detonating can trigger the ninja's objective.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedSpiderChargeSystem))]
public sealed partial class SpiderChargeComponent : Component
{
    /// Range for planting within the target area
    [DataField]
    public float Range = 10f;

    /// The ninja that planted this charge
    [DataField]
    public EntityUid? Planter;
}
