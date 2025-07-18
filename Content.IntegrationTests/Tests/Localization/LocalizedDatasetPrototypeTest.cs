// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Dataset;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Localization;

[TestFixture]
public sealed class LocalizedDatasetPrototypeTest
{
    [Test]
    public async Task ValidProtoIdsTest()
    {
        await using var pair = await PoolManager.GetServerClient();

        var server = pair.Server;
        var protoMan = server.ResolveDependency<IPrototypeManager>();
        var localizationMan = server.ResolveDependency<ILocalizationManager>();

        var protos = protoMan.EnumeratePrototypes<LocalizedDatasetPrototype>().OrderBy(p => p.ID);

        Assert.Multiple(() =>
        {
            // Check each prototype
            foreach (var proto in protos)
            {
                // Check each value in the prototype
                foreach (var locId in proto.Values)
                {
                    // Make sure the localization manager has a string for the LocId
                    Assert.That(localizationMan.HasString(locId), $"LocalizedDataset {proto.ID} with prefix \"{proto.Values.Prefix}\" specifies {proto.Values.Count} entries, but no localized string was found matching {locId}!");
                }
            }
        });

        await pair.CleanReturnAsync();
    }
}
