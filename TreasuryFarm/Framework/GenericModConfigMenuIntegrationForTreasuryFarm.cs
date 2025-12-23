using DonCami.Stardew.Common.Integrations.GenericModConfigMenu;
using DonCami.Stardew.TreasuryFarm.Framework.Config;
using StardewModdingAPI;

namespace DonCami.Stardew.TreasuryFarm.Framework;

/// <summary>Registers the mod configuration with Generic Mod Config Menu.</summary>
internal class GenericModConfigMenuIntegrationForTreasuryFarm : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    /*********
     ** Public methods
     *********/
    /// <summary>Register the config menu if available.</summary>
    public void Register(GenericModConfigMenuIntegration<ModConfig> menu, IMonitor monitor)
    {
        // register
        menu
            .Register(true) // configuring in-game would have unintended effects like fores farm logic being half-applied

            // farm options
            .AddSectionTitle(I18n.Config_FarmOptionsSection)
            .AddCheckbox(
                I18n.Config_SpawnMonsters_Name,
                I18n.Config_SpawnMonsters_Tooltip,
                config => config.DefaultSpawnMonstersAtNight,
                (config, value) => config.DefaultSpawnMonstersAtNight = value
            );
    }
}