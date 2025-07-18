// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Beam.Components;
using Content.Shared.Beam;
using Content.Shared.Beam.Components;
using Robust.Client.GameObjects;

namespace Content.Client.Beam;

public sealed class BeamSystem : SharedBeamSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<BeamVisualizerEvent>(BeamVisualizerMessage);
    }

    //TODO: Sometime in the future this needs to be replaced with tiled sprites
    private void BeamVisualizerMessage(BeamVisualizerEvent args)
    {
        var beam = GetEntity(args.Beam);

        if (TryComp<SpriteComponent>(beam, out var sprites))
        {
            sprites.Rotation = args.UserAngle;

            if (args.BodyState != null)
            {
                sprites.LayerSetState(0, args.BodyState);
                sprites.LayerSetShader(0, args.Shader);
            }
        }
    }
}
