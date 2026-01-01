namespace DonCami.Stardew.TreasuryFarm.Framework.Config;

/// <summary>The mod configuration.</summary>
internal class ModConfig
{
    /*********
     ** Accessors
     *********/
    /****
     ** farm options
     ****/

    /// <summary>The default value for the 'spawn monsters at night' option when creating a new save.</summary>
    public bool DefaultSpawnMonstersAtNight { get; set; } = false;
    
    /// <summary>The default value for the 'spawn stumps' option when creating a new save.</summary>

    public bool DefaultSpawnStumps { get; set; } = true;
}