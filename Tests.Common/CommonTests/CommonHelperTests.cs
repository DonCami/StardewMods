using System.IO;
using DonCami.Stardew.Common;
using NUnit.Framework;

namespace DonCami.Stardew.Tests.Common.CommonTests;

/// <summary>Unit tests for <see cref="CommonHelper" />.</summary>
[TestFixture]
internal class CommonHelperTests
{
    /*********
     ** Unit tests
     *********/
    /****
     ** GetFileHash
     ****/
    [Test(Description =
        $"Assert that {nameof(CommonHelper.GetFileHash)} throws FileHash")]
    [TestCase(
        @"H:\projects\StardewValley\MyMods\StardewModsRider\StardewMods\TreasuryFarm",
        ExpectedResult = "2f68f6b6df33c0fb888ae5c09de91e69"
        )]
    public string GetFormattedPercentageNumber_OutOfRangeValues_SnapToValidValue(string filePath)
    {
        var dataPath = Path.Combine(filePath, "assets", "data.json");
        return CommonHelper.GetFileHash(dataPath);
    }
    
}