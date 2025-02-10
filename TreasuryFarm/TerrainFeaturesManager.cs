using System.Collections.Generic;
using System.Linq;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DonCami.Stardew.TreasuryFarm;

public static class TerrainFeaturesManager
{
    private const string ExpansionOneName = "DonCami.TreasuryFarm.ExtensionOne";
    
    public static void CheckAndManageBushes()
    {
        var farmLocation = Game1.getLocationFromName("Farm");
        var expansionOne = Game1.getLocationFromName(ExpansionOneName);
        
        if (farmLocation == null) return;
        if (expansionOne != null)
        {
            RemoveBushes(farmLocation, FarmToExpansionOneBushes());
            AddWarps(farmLocation, FarmToExpansionOneWarps());
        }
        else
        {
            AddBushes(farmLocation, FarmToExpansionOneBushes());
        }
    }

    private static void RemoveBushes(GameLocation location, List<NetVector2> bushes)
    {
        foreach (var bush in bushes
                     .Select(
                         bushTile => 
                             location.largeTerrainFeatures
                                 .OfType<Bush>()
                                 .FirstOrDefault(b => b.netTilePosition == bushTile)
                     )
                     .OfType<Bush>()
                )
        {
            location.largeTerrainFeatures.Remove(bush);
        }
    }
    
    private static void AddBushes(GameLocation location, List<NetVector2> bushes)
    {
        foreach (var bushTile in from bushTile in bushes let bush = location.largeTerrainFeatures
                     .OfType<Bush>()
                     .FirstOrDefault(b => b.netTilePosition == bushTile) where bush == null select bushTile)
        {
            location.largeTerrainFeatures.Add(new Bush(bushTile.Value,1,location,1));
        }
    }
    
    
    
    private static List<Warp> FarmToExpansionOneWarps()
    {
        return
        [
            new Warp(-1, 24, ExpansionOneName, 154, 105, false),
            new Warp(-1, 25, ExpansionOneName, 154, 106, false),
            new Warp(-1, 26, ExpansionOneName, 154, 107, false)
        ];
    }

    private static List<NetVector2> FarmToExpansionOneBushes()
    {
        return
            [
            new NetVector2(new Microsoft.Xna.Framework.Vector2(1, 24)),
            new NetVector2(new Microsoft.Xna.Framework.Vector2(1, 25)),
            new NetVector2(new Microsoft.Xna.Framework.Vector2(1, 26)),
            new NetVector2(new Microsoft.Xna.Framework.Vector2(2, 24)),
            new NetVector2(new Microsoft.Xna.Framework.Vector2(2, 25)),
            new NetVector2(new Microsoft.Xna.Framework.Vector2(2, 26))
        ];
    }
    
    private static void AddWarps(GameLocation location, List<Warp> warps)
    {
        foreach (var warp in warps)
        {
            location.warps.Add(warp);
        }
    }
}