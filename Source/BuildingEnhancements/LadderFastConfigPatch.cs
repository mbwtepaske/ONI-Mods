using Harmony;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(LadderFastConfig), nameof(LadderConfig.CreateBuildingDef))]
  public static class LadderFastConfigPatch
  {
    public static void Postfix(BuildingDef __result)
    {
      __result.ReplacementLayer = ObjectLayer.ReplacementTravelTube;
      __result.TileLayer = ObjectLayer.TravelTube;
    }
  }
}