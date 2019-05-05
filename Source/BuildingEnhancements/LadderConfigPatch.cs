using Harmony;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(LadderConfig), nameof(LadderConfig.CreateBuildingDef))]
  public static class LadderConfigPatch
  {
    public static void Postfix(BuildingDef __result)
    {
      __result.ReplacementLayer = ObjectLayer.ReplacementTravelTube;
      __result.TileLayer = ObjectLayer.TravelTube;
    }
  }
}
