// SPDX-FileCopyrightText: 2022 Javier Guardia Fernández <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2022 github-actions <github-actions@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.IntegrationTests;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Robust.Client.GameObjects;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Content.MapRenderer.Painters
{
    public sealed class MapPainter
    {
        public static async IAsyncEnumerable<RenderedGridImage<Rgba32>> Paint(string map)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await using var pair = await PoolManager.GetServerClient(new PoolSettings
            {
                DummyTicker = false,
                Connected = true,
                Fresh = true,
                // Seriously whoever made MapPainter use GameMapPrototype I wish you step on a lego one time.
                Map = map,
            });

            var server = pair.Server;
            var client = pair.Client;

            Console.WriteLine($"Loaded client and server in {(int) stopwatch.Elapsed.TotalMilliseconds} ms");

            stopwatch.Restart();

            var cEntityManager = client.ResolveDependency<IClientEntityManager>();
            var cPlayerManager = client.ResolveDependency<Robust.Client.Player.IPlayerManager>();

            await client.WaitPost(() =>
            {
                if (cEntityManager.TryGetComponent(cPlayerManager.LocalEntity, out SpriteComponent? sprite))
                {
                    sprite.Visible = false;
                }
            });

            var sEntityManager = server.ResolveDependency<IServerEntityManager>();
            var sPlayerManager = server.ResolveDependency<IPlayerManager>();

            await pair.RunTicksSync(10);
            await Task.WhenAll(client.WaitIdleAsync(), server.WaitIdleAsync());

            var sMapManager = server.ResolveDependency<IMapManager>();

            var tilePainter = new TilePainter(client, server);
            var entityPainter = new GridPainter(client, server);
            Entity<MapGridComponent>[] grids = null!;
            var xformQuery = sEntityManager.GetEntityQuery<TransformComponent>();
            var xformSystem = sEntityManager.System<SharedTransformSystem>();

            await server.WaitPost(() =>
            {
                var playerEntity = sPlayerManager.Sessions.Single().AttachedEntity;

                if (playerEntity.HasValue)
                {
                    sEntityManager.DeleteEntity(playerEntity.Value);
                }

                var mapId = sEntityManager.System<GameTicker>().DefaultMap;
                grids = sMapManager.GetAllGrids(mapId).ToArray();

                foreach (var (uid, _) in grids)
                {
                    var gridXform = xformQuery.GetComponent(uid);
                    xformSystem.SetWorldRotation(gridXform, Angle.Zero);
                }
            });

            await pair.RunTicksSync(10);
            await Task.WhenAll(client.WaitIdleAsync(), server.WaitIdleAsync());

            foreach (var (uid, grid) in grids)
            {
                // Skip empty grids
                if (grid.LocalAABB.IsEmpty())
                {
                    Console.WriteLine($"Warning: Grid {uid} was empty. Skipping image rendering.");
                    continue;
                }

                var tileXSize = grid.TileSize * TilePainter.TileImageSize;
                var tileYSize = grid.TileSize * TilePainter.TileImageSize;

                var bounds = grid.LocalAABB;

                var left = bounds.Left;
                var right = bounds.Right;
                var top = bounds.Top;
                var bottom = bounds.Bottom;

                var w = (int) Math.Ceiling(right - left) * tileXSize;
                var h = (int) Math.Ceiling(top - bottom) * tileYSize;

                var gridCanvas = new Image<Rgba32>(w, h);

                await server.WaitPost(() =>
                {
                    tilePainter.Run(gridCanvas, uid, grid);
                    entityPainter.Run(gridCanvas, uid, grid);

                    gridCanvas.Mutate(e => e.Flip(FlipMode.Vertical));
                });

                var renderedImage = new RenderedGridImage<Rgba32>(gridCanvas)
                {
                    GridUid = uid,
                    Offset = xformSystem.GetWorldPosition(uid),
                };

                yield return renderedImage;
            }

            // We don't care if it fails as we have already saved the images.
            try
            {
                await pair.CleanReturnAsync();
            }
            catch
            {
                // ignored
            }
        }
    }
}
