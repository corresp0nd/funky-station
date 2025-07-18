// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Speech;
using Robust.Shared.Prototypes;

namespace Content.Server.Speech.Components;

/// <summary>
///     Will change the voice of the entity that has the component (e.g radio and speech).
/// </summary>
/// <remarks>
///     Before using this component, please take a look at the the TransformSpeakerNameEvent (and the inventory relay version).
///     Depending on what you're doing, it could be a better choice!
/// </remarks>
[RegisterComponent]
public sealed partial class VoiceOverrideComponent : Component
{
    /// <summary>
    ///     The name that will be used instead of an entities default one.
    ///     Uses the localized version of the string and if null wont do anything.
    /// </summary>
    [DataField]
    public string? NameOverride = null;

    /// <summary>
    ///     The verb that will be used insteand of an entities default one.
    ///     If null, the defaut will be used.
    /// </summary>
    [DataField]
    public ProtoId<SpeechVerbPrototype>? SpeechVerbOverride = null;

    /// <summary>
    ///     If true, the override values (if they are not null) will be applied.
    /// </summary>
    [DataField]
    public bool Enabled = true;
}
