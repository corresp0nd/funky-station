// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Access.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Access.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(ActivatableUIRequiresAccessSystem))]
public sealed partial class ActivatableUIRequiresAccessComponent : Component
{
    [DataField]
    public LocId? PopupMessage = "lock-comp-has-user-access-fail";
}
