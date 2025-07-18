// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.NPC.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.NPC.Components;

/// <summary>
/// This is used for tracking entities stored in <see cref="FactionExceptionComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(NpcFactionSystem))]
public sealed partial class FactionExceptionTrackerComponent : Component
{
    /// <summary>
    /// Entities with <see cref="FactionExceptionComponent"/> that are tracking this entity.
    /// </summary>
    [DataField]
    public HashSet<EntityUid> Entities = new();
}
