using System;
using System.Diagnostics.CodeAnalysis;
using DonCami.Stardew.Common.Patching;
using DonCami.Stardew.TreasuryFarm.Framework.Config;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace DonCami.Stardew.TreasuryFarm.Patches;

/// <summary>Encapsulates Harmony patches for the <see cref="Farm" /> instance.</summary>
internal class FarmPatcher : BasePatcher
{
    /*********
     ** Fields
     *********/
    /// <summary>The mod configuration.</summary>
    private static ModConfig Config = null!; // set in constructor

    /// <summary>Get whether the given location is the Giga Farm.</summary>
    private static Func<GameLocation?, bool> IsTreasuryFarm = null!; // set in constructor


    /*********
     ** Public methods
     *********/
    /// <summary>Initialize the patcher.</summary>
    /// <param name="config">The mod configuration.</param>
    /// <param name="isTreasuryFarm">Get whether the given location is the Giga Farm.</param>
    public FarmPatcher(ModConfig config, Func<GameLocation?, bool> isTreasuryFarm)
    {
        Config = config;
        IsTreasuryFarm = isTreasuryFarm;
    }

    /// <inheritdoc />
    public override void Apply(Harmony harmony, IMonitor monitor)
    {
        harmony.Patch(
            RequireMethod<Farm>("resetLocalState"),
            postfix: GetHarmonyMethod(nameof(After_ResetLocalState))
        );
        harmony.Patch(
            RequireMethod<Farm>("resetSharedState"),
            postfix: GetHarmonyMethod(nameof(After_ResetSharedState))
        );
        harmony.Patch(
            RequireMethod<GameLocation>(nameof(GameLocation.cleanupBeforePlayerExit)),
            postfix: GetHarmonyMethod(nameof(After_CleanupBeforePlayerExit))
        );
        harmony.Patch(
            RequireMethod<GameLocation>(nameof(GameLocation.getRandomTile)),
            postfix: GetHarmonyMethod(nameof(After_GetRandomTile))
        );
    }


    /*********
     ** Private methods
     *********/
    /// <summary>A method called via Harmony after <see cref="Farm.resetLocalState" />.</summary>
    /// <param name="__instance">The farm instance.</param>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
    private static void After_ResetLocalState(GameLocation __instance)
    {
        if (!IsTreasuryFarm(__instance))
            return;
    }

    /// <summary>A method called via Harmony after <see cref="Farm.resetSharedState" />.</summary>
    /// <param name="__instance">The farm instance.</param>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
    private static void After_ResetSharedState(GameLocation __instance)
    {
        if (!IsTreasuryFarm(__instance))
            return;
    }

    /// <summary>A method called via Harmony after <see cref="Farm.cleanupBeforePlayerExit" />.</summary>
    /// <param name="__instance">The farm instance.</param>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
    private static void After_CleanupBeforePlayerExit(GameLocation __instance)
    {
        if (!IsTreasuryFarm(__instance))
            return;
    }

    /// <summary>A method called via Harmony after <see cref="GameLocation.getRandomTile" />.</summary>
    /// <param name="__instance">The farm instance.</param>
    /// <param name="__result">The return value to use for the method.</param>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
    private static void After_GetRandomTile(GameLocation __instance, ref Vector2 __result)
    {
        if (!IsTreasuryFarm(__instance))
            return;
    }
}