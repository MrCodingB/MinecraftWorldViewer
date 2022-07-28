namespace Minecraft;

public class Biome
{
    public readonly struct BiomeInfo
    {
        public readonly string Name;

        public readonly decimal Temperature;

        public readonly decimal Rainfall;

        public BiomeInfo(string name, decimal temperature, decimal rainfall)
        {
            Name = name;
            Rainfall = rainfall;
            Temperature = temperature;
        }
    }

    private static readonly BiomeInfo[] BiomeInfoList =
    {
        new(Biomes.Badlands, 2, 0),
        new(Biomes.WoodedBadlands, 2, 0),
        new(Biomes.ErodedBadlands, 2, 0),
        new(Biomes.Desert, 2, 0),
        new(Biomes.Savanna, 1.2M, 0),
        new(Biomes.WindsweptSavanna, 1.1M, 0),
        new(Biomes.SavannaPlateau, 1, 0),
        new(Biomes.StonyPeaks, 1, 0.3M),
        new(Biomes.Jungle, 0.95M, 0.9M),
        new(Biomes.BambooJungle, 0.95M, 0.9M),
        new(Biomes.SparseJungle, 0.95M, 0.8M),
        new(Biomes.MushroomFields, 0.9M, 1),
        new(Biomes.Plains, 0.8M, 0.4M),
        new(Biomes.SunflowerPlains, 0.8M, 0.4M),
        new(Biomes.Beach, 0.8M, 0.4M),
        new(Biomes.Swamp, 0.8M, 0.9M),
        new(Biomes.MangroveSwamp, 0.8M, 0.9M),
        new(Biomes.DeepDark, 0.8M, 0.4M),
        new(Biomes.DripstoneCaves, 0.8M, 0.4M),
        new(Biomes.DarkForest, 0.7M, 0.8M),
        new(Biomes.Forest, 0.7M, 0.8M),
        new(Biomes.FlowerForest, 0.7M, 0.8M),
        new(Biomes.OldGrowthBirchForest, 0.6M, 0.6M),
        new(Biomes.BirchForest, 0.6M, 0.6M),
        new(Biomes.LushCaves, 0.5M, 0.5M),
        new(Biomes.ColdOcean, 0.5M, 0.5M),
        new(Biomes.DeepColdOcean, 0.5M, 0.5M),
        new(Biomes.DeepFrozenOcean, 0.5M, 0.5M),
        new(Biomes.DeepLukewarmOcean, 0.5M, 0.5M),
        new(Biomes.DeepOcean, 0.5M, 0.5M),
        new(Biomes.LukewarmOcean, 0.5M, 0.5M),
        new(Biomes.Ocean, 0.5M, 0.5M),
        new(Biomes.River, 0.5M, 0.5M),
        new(Biomes.WarmOcean, 0.5M, 0.5M),
        new(Biomes.Meadow, 0.5M, 0.8M),
        new(Biomes.OldGrowthPineTaiga, 0.3M, 0.8M),
        new(Biomes.Taiga, 0.25M, 0.8M),
        new(Biomes.OldGrowthSpruceTaiga, 0.25M, 0.8M),
        new(Biomes.WindsweptHills, 0.2M, 0.3M),
        new(Biomes.WindsweptForest, 0.2M, 0.3M),
        new(Biomes.WindsweptGravellyHills, 0.2M, 0.3M),
        new(Biomes.StonyShore, 0.2M, 0.3M),
        new(Biomes.SnowyBeach, 0.05M, 0.3M),
        new(Biomes.SnowyPlains, 0, 0.5M),
        new(Biomes.IceSpikes, 0, 0.5M),
        new(Biomes.FrozenRiver, 0, 0.5M),
        new(Biomes.FrozenOcean, 0, 0.5M),
        new(Biomes.Grove, -0.2M, 0.8M),
        new(Biomes.SnowySlopes, -0.3M, 0.9M),
        new(Biomes.SnowyTaiga, -0.5M, 0.4M),
        new(Biomes.JaggedPeaks, -0.7M, 0.9M),
        new(Biomes.FrozenPeaks, -0.7M, 0.9M)
    };

    public static readonly IDictionary<string, BiomeInfo> BiomeInfos =
        BiomeInfoList.ToDictionary(b => b.Name, b => b);
}
