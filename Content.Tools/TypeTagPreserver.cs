// SPDX-FileCopyrightText: 2021 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Content.Tools
{
    public sealed class TypeTagPreserver : IEmitter
    {
        public TypeTagPreserver(IEmitter emitter)
        {
            Emitter = emitter;
        }

        private IEmitter Emitter { get; }

        public void Emit(ParsingEvent @event)
        {
            if (@event is MappingStart mapping)
            {
                @event = new MappingStart(mapping.Anchor, mapping.Tag, false, mapping.Style, mapping.Start, mapping.End);
            }

            Emitter.Emit(@event);
        }
    }
}
