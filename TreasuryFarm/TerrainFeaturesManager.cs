using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DonCami.Stardew.TreasuryFarm;

public static class TerrainFeaturesManager
{
    private const string ExtensionOneName = "DonCami.TreasuryFarm.ExtensionOne";
    private const string ExtensionTwoName = "DonCami.TreasuryFarm.ExtensionTwo";

    public static void CheckAndManageBushes()
    {
        var farmLocation = Game1.getLocationFromName("Farm");
        var extensionOne = Game1.getLocationFromName(ExtensionOneName);
        var extensionTwo = Game1.getLocationFromName(ExtensionTwoName);

        if (farmLocation == null) return;
        if (extensionOne != null)
        {
            RemoveBushes(farmLocation, FarmToExpansionOneBushes());
        }
        else
        {
            AddBushes(farmLocation, FarmToExpansionOneBushes());
        }
        if (extensionTwo != null)
        {
            RemoveBushes(farmLocation, FarmToExpansionTwoWarpZone());
        }
        else
        {
            AddBushes(farmLocation, FarmToExpansionTwoBushesDefault());
        }
    }
    
    public static void RemoveStumps()
    {
        var farmLocation = Game1.getLocationFromName("Farm");
        if (farmLocation == null) return;

        RemoveStumpsProperty(farmLocation);
    }

    private static void RemoveBushes(GameLocation location, List<NetVector2> bushes)
    {
        foreach (var bush in bushes
                     .Select(bushTile =>
                         location.largeTerrainFeatures
                             .OfType<Bush>()
                             .FirstOrDefault(b => b.netTilePosition == bushTile)
                     )
                     .OfType<Bush>()
                )
            location.largeTerrainFeatures.Remove(bush);
    }

    private static void AddBushes(GameLocation location, List<NetVector2> bushes)
    {
        foreach (var bushTile in from bushTile in bushes
                 let bush = location.largeTerrainFeatures
                     .OfType<Bush>()
                     .FirstOrDefault(b => b.netTilePosition == bushTile)
                 where bush == null
                 select bushTile)
            location.largeTerrainFeatures.Add(new Bush(bushTile.Value, 1, location, 1));
    }
    
    /// <summary>
    /// Removes the Stumps property from the specified location's map.
    /// </summary>
    /// <param name="location">The game location whose map property should be modified.</param>
    public static void RemoveStumpsProperty(GameLocation location)
    {
        if (location?.map?.Properties == null) return;
        
        location.map.Properties.Remove("Stumps");
    }
    
    private static List<NetVector2> FarmToExpansionOneBushes()
    {
        return
        [
            new NetVector2(new Vector2(1, 24)),
            new NetVector2(new Vector2(1, 25)),
            new NetVector2(new Vector2(1, 26)),
            new NetVector2(new Vector2(2, 24)),
            new NetVector2(new Vector2(2, 25)),
            new NetVector2(new Vector2(2, 26))
        ];
    }
    
    private static List<NetVector2> FarmToExpansionTwoBushesDefault()
    {
        return
        [
            new NetVector2(new Vector2(2, 123)),
            new NetVector2(new Vector2(2, 124)),
            new NetVector2(new Vector2(2, 125)),
            new NetVector2(new Vector2(2, 126)),
        ];
    }
    
    private static List<NetVector2> FarmToExpansionTwoWarpZone()
    {
        return
        [
            new NetVector2(new Vector2(1, 123)),
            new NetVector2(new Vector2(1, 124)),
            new NetVector2(new Vector2(1, 125)),
            new NetVector2(new Vector2(2, 123)),
            new NetVector2(new Vector2(2, 124)),
            new NetVector2(new Vector2(2, 125)),
            new NetVector2(new Vector2(3, 123)),
            new NetVector2(new Vector2(3, 124)),
            new NetVector2(new Vector2(3, 125))
        ];
    }
}