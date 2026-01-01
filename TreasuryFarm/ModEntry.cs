using System.Collections.Generic;
using System.IO;
using DonCami.Stardew.Common;
using DonCami.Stardew.Common.Integrations.GenericModConfigMenu;
using DonCami.Stardew.Common.Patching;
using DonCami.Stardew.TreasuryFarm.Framework;
using DonCami.Stardew.TreasuryFarm.Framework.Config;
using DonCami.Stardew.TreasuryFarm.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Locations;
using xTile;

namespace DonCami.Stardew.TreasuryFarm;

/// <summary>The mod entry class loaded by SMAPI.</summary>
public class ModEntry : Mod
{
    /*********
     ** Fields
     *********/
    /// <summary>The MD5 hash for the default data.json file.</summary>
    private const string DataFileHash = "2f68f6b6df33c0fb888ae5c09de91e69";

    /// <summary>The mod configuration.</summary>
    private ModConfig Config = null!; // set in Entry

    /// <summary>The mod's hardcoded data.</summary>
    private ModData Data = null!; // set in Entry


    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);

        // read config & data
        Config = Helper.ReadConfig<ModConfig>();
        Data = LoadModData();

        // hook events
        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

        // hook Harmony patch
        HarmonyPatcher.Apply(this,
            new FarmPatcher(
                Config,
                IsTreasuryFarm
            ),
            new CharacterCustomizationPatcher(
                Config,
                ModManifest.UniqueID
            )
        );
    }


    /*********
     ** Private methods
     *********/
    /// <inheritdoc cref="IContentEvents.AssetRequested" />
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        const string farmKey = "DonCami_TreasuryFarm";

        // add farm type
        if (e.NameWithoutLocale.IsEquivalentTo("Data/AdditionalFarms"))
            e.Edit(editor =>
            {
                var data = editor.GetData<List<ModFarmType>>();
                data.Add(new ModFarmType
                {
                    Id = ModManifest.UniqueID,
                    TooltipStringPath = $"Strings/UI:{farmKey}_Description",
                    MapName = farmKey
                });
            });

        // add farm description
        else if (e.NameWithoutLocale.IsEquivalentTo("Strings/UI"))
            e.Edit(editor =>
            {
                var data = editor.AsDictionary<string, string>().Data;

                // key used by the title screen
                data[$"{farmKey}_Description"] = $"{I18n.Farm_Name()}_{I18n.Farm_Description()}";

                // custom keys used in data.json
                data[$"{farmKey}_Name"] = I18n.Farm_Name();
            });

        // add farm location data
        else if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations") && Data.LocationData != null)
            e.Edit(editor =>
            {
                var data = editor.AsDictionary<string, LocationData>().Data;
                data[$"Farm_{ModManifest.UniqueID}"] = Data.LocationData;
            });

        // load map
        else if (e.NameWithoutLocale.IsEquivalentTo("Maps/DonCami_TreasuryFarm"))
            e.LoadFrom(
                () =>
                {
                    // load map
                    var map = Helper.ModContent.Load<Map>("assets/treasuryfarm.tmx");

                    return map;
                },
                AssetLoadPriority.Exclusive
            );
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched" />
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForTreasuryFarm(),
            () => Config,
            config => Config = config
        );
    }

    /// <summary>Get whether the given location is the Treasury Farm.</summary>
    /// <param name="location">The location to check.</param>
    private bool IsTreasuryFarm(GameLocation? location)
    {
        return
            Game1.whichModFarm?.Id == ModManifest.UniqueID
            && location?.Name == "Farm"
            && location is Farm;
    }

    /// <summary>Load the mod data from the <c>assets/data.json</c> file.</summary>
    private ModData LoadModData()
    {
        var data = Helper.Data.ReadJsonFile<ModData>("assets/data.json");

        var dataPath = Path.Combine(Helper.DirectoryPath, "assets", "data.json");
        if (data == null || !File.Exists(dataPath))
        {
            Monitor.Log(
                "The mod's 'assets/data.json' file is missing, so this mod can't work correctly. Please reinstall the mod to fix ",
                LogLevel.Error);
            return new ModData();
        }

        if (CommonHelper.GetFileHash(dataPath) != DataFileHash)
            Monitor.Log("Found edits to 'assets/data.json'.");

        if (data.LocationData is not null) return data;
        Monitor.Log(
            "The mod's 'assets/data.json' file has invalid location data, so this mod can't work correctly. Please reinstall the mod to fix this.",
            LogLevel.Error);
        data.LocationData = null;

        return data;
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded" />
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs saveLoadedEventArgs)
    {
        TerrainFeaturesManager.CheckAndManageBushes();
        var extensionOne = Game1.getLocationFromName("DonCami.TreasuryFarm.ExtensionOne");
        var extensionTwo = Game1.getLocationFromName("DonCami.TreasuryFarm.ExtensionTwo");
        if (extensionOne != null)
            Monitor.Log(
                "Treasury Farm Extension One loaded successfully.",
                LogLevel.Info);
        if (extensionTwo != null)
            Monitor.Log(
                "Treasury Farm Extension Two loaded successfully.",
                LogLevel.Info);
    }
}