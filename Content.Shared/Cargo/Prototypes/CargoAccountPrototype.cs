// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 jackel234 <52829582+jackel234@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo.Prototypes;

/// <summary>
/// This is a prototype for a single account that stores money on StationBankAccountComponent
/// </summary>
[Prototype]
public sealed partial class CargoAccountPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Full IC name of the account.
    /// </summary>
    [DataField]
    public LocId Name;

    /// <summary>
    /// A shortened code used to refer to the account in UIs
    /// </summary>
    [DataField]
    public LocId Code;

    /// <summary>
    /// Color corresponding to the account.
    /// </summary>
    [DataField]
    public Color Color;

    /// <summary>
    /// Channel used for announcing transactions.
    /// </summary>
    [DataField]
    public ProtoId<RadioChannelPrototype> RadioChannel;

    /// <summary>
    /// Paper prototype used for acquisition slips.
    /// </summary>
    [DataField]
    public EntProtoId AcquisitionSlip;
}
