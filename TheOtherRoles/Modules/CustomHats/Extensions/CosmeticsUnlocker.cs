namespace TheOtherRoles.Modules.CustomHats.Extensions;

public static class CosmeticsUnlocker
{
    public static void unlockCosmetics(HatManager hatManager)
    {
        foreach (BundleData bundleData in hatManager.allBundles)
        {
            bundleData.Free = true;
        }
        foreach (BundleData bundleData2 in hatManager.allFeaturedBundles)
        {
            bundleData2.Free = true;
        }
        foreach (CosmicubeData cosmicubeData in hatManager.allFeaturedCubes)
        {
            cosmicubeData.Free = true;
        }
        foreach (CosmeticData cosmeticData in hatManager.allFeaturedItems)
        {
            cosmeticData.Free = true;
        }
        foreach (HatData hatData in hatManager.allHats)
        {
            hatData.Free = true;
        }
        foreach (NamePlateData namePlateData in hatManager.allNamePlates)
        {
            namePlateData.Free = true;
        }
        foreach (PetData petData in hatManager.allPets)
        {
            petData.Free = true;
        }
        foreach (SkinData skinData in hatManager.allSkins)
        {
            skinData.Free = true;
        }
        foreach (StarBundle starBundle in hatManager.allStarBundles)
        {
            starBundle.price = 0f;
        }
        foreach (VisorData visorData in hatManager.allVisors)
        {
            visorData.Free = true;
        }
    }
}