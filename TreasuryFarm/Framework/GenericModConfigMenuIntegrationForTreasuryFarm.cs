using System;
using DonCami.Stardew.TreasuryFarm.Framework.Config;
using Pathoschild.Stardew.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;

namespace DonCami.Stardew.TreasuryFarm.Framework
{
    /// <summary>Registers the mod configuration with Generic Mod Config Menu.</summary>
    internal class GenericModConfigMenuIntegrationForTreasuryFarm
    {
        /*********
        ** Fields
        *********/
        /// <summary>The Generic Mod Config Menu integration.</summary>
        private readonly GenericModConfigMenuIntegration<ModConfig> ConfigMenu;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="manifest">The mod manifest.</param>
        /// <param name="getConfig">Get the current config model.</param>
        /// <param name="reset">Reset the config model to the default values.</param>
        /// <param name="saveAndApply">Save and apply the current config model.</param>
        public GenericModConfigMenuIntegrationForTreasuryFarm(IModRegistry modRegistry, IMonitor monitor, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action saveAndApply)
        {
            this.ConfigMenu = new GenericModConfigMenuIntegration<ModConfig>(modRegistry, monitor, manifest, getConfig, reset, saveAndApply);
        }

        /// <summary>Register the config menu if available.</summary>
        public void Register()
        {
            // get config menu
            if (!ConfigMenu.IsLoaded)
                return;

            // register
            ConfigMenu
                .Register(titleScreenOnly: true) // configuring in-game would have unintended effects like fores farm logic being half-applied

                // farm options
                .AddSectionTitle(I18n.Config_FarmOptionsSection)
                .AddCheckbox(
                    name: I18n.Config_SpawnMonsters_Name,
                    tooltip: I18n.Config_SpawnMonsters_Tooltip,
                    get: config => config.DefaultSpawnMonstersAtNight,
                    set: (config, value) => config.DefaultSpawnMonstersAtNight = value
                );


        }
    }
}
