// SPDX-FileCopyrightText: 2021 Paul <ritter.paul1+git@googlemail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Holiday.Greet;
using Content.Server.Holiday.Interfaces;
using Content.Server.Holiday.ShouldCelebrate;
using Robust.Shared.Prototypes;

namespace Content.Server.Holiday
{
    [Prototype]
    public sealed partial class HolidayPrototype : IPrototype
    {
        [DataField("name")] public string Name { get; private set; } = string.Empty;

        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField("beginDay")]
        public byte BeginDay { get; set; } = 1;

        [DataField("beginMonth")]
        public Month BeginMonth { get; set; } = Month.Invalid;

        /// <summary>
        ///     Day this holiday will end. Zero means it lasts a single day.
        /// </summary>
        [DataField("endDay")]
        public byte EndDay { get; set; }

        /// <summary>
        ///     Month this holiday will end in. Invalid means it lasts a single month.
        /// </summary>
        [DataField("endMonth")]
        public Month EndMonth { get; set; } = Month.Invalid;

        [DataField("shouldCelebrate")]
        private IHolidayShouldCelebrate _shouldCelebrate = new DefaultHolidayShouldCelebrate();

        [DataField("greet")]
        private IHolidayGreet _greet = new DefaultHolidayGreet();

        [DataField("celebrate")]
        private IHolidayCelebrate? _celebrate = null;

        public bool ShouldCelebrate(DateTime date)
        {
            return _shouldCelebrate.ShouldCelebrate(date, this);
        }

        public string Greet()
        {
            return _greet.Greet(this);
        }

        /// <summary>
        ///     Called before the round starts to set up any festive shenanigans.
        /// </summary>
        public void Celebrate()
        {
            _celebrate?.Celebrate(this);
        }
    }
}
