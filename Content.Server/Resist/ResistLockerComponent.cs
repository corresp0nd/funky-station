// SPDX-FileCopyrightText: 2022 JustinTime <41876089+JustinTether@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Threading;

namespace Content.Server.Resist;

[RegisterComponent]
[Access(typeof(ResistLockerSystem))]
public sealed partial class ResistLockerComponent : Component
{
    /// <summary>
    /// How long will this locker take to kick open, defaults to 2 minutes
    /// </summary>
    [DataField("resistTime")]
    public float ResistTime = 120f;

    /// <summary>
    /// For quick exit if the player attempts to move while already resisting
    /// </summary>
    [ViewVariables]
    public bool IsResisting = false;
}
