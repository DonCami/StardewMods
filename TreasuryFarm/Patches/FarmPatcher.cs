using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common.Patching;
using DonCami.Stardew.TreasuryFarm.Framework.Config;
using StardewModdingAPI;
using StardewValley;

namespace DonCami.Stardew.TreasuryFarm.Patches
{
    /// <summary>Encapsulates Harmony patches for the <see cref="Farm"/> instance.</summary>
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
            FarmPatcher.Config = config;
            FarmPatcher.IsTreasuryFarm = isTreasuryFarm;
        }

        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Farm>("resetLocalState"),
                postfix: this.GetHarmonyMethod(nameof(FarmPatcher.After_ResetLocalState))
            );
            harmony.Patch(
                original: this.RequireMethod<Farm>("resetSharedState"),
                postfix: this.GetHarmonyMethod(nameof(FarmPatcher.After_ResetSharedState))
            );
            harmony.Patch(
                original: this.RequireMethod<GameLocation>(nameof(GameLocation.cleanupBeforePlayerExit)),
                postfix: this.GetHarmonyMethod(nameof(FarmPatcher.After_CleanupBeforePlayerExit))
            );
            harmony.Patch(
                original: this.RequireMethod<GameLocation>(nameof(GameLocation.getRandomTile)),
                postfix: this.GetHarmonyMethod(nameof(FarmPatcher.After_GetRandomTile))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>A method called via Harmony after <see cref="Farm.resetLocalState"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_ResetLocalState(GameLocation __instance)
        {
            if (!FarmPatcher.IsTreasuryFarm(__instance))
                return;
            
        }

        /// <summary>A method called via Harmony after <see cref="Farm.resetSharedState"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_ResetSharedState(GameLocation __instance)
        {
            if (!IsTreasuryFarm(__instance))
                return;
        }

        /// <summary>A method called via Harmony after <see cref="Farm.cleanupBeforePlayerExit"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_CleanupBeforePlayerExit(GameLocation __instance)
        {
            if (!FarmPatcher.IsTreasuryFarm(__instance))
                return;
            
        }

        /// <summary>A method called via Harmony after <see cref="GameLocation.getRandomTile"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        /// <param name="__result">The return value to use for the method.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_GetRandomTile(GameLocation __instance, ref Vector2 __result)
        {
            if (!FarmPatcher.IsTreasuryFarm(__instance))
                return;
            
        }
        
    }
}
