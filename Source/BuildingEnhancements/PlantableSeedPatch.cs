using HarmonyLib;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(PlantableSeed), "TestSuitableGround")]
  public static class PlantableSeedTestingPatch
  {
    [HarmonyPostfix]
    public static void Fix(PlantableSeed __instance, int cell, ref bool __result)
    {
      var prefab = Assets.GetPrefab(__instance.PlantID);
      
      if (!__result)
      {
        var cellBelow = Grid.CellBelow(cell);

        __result = Grid.IsValidCell(cell) 
          && Grid.IsValidCell(cellBelow) 
          && !Grid.Foundation[cellBelow]
          && Grid.Element[cellBelow].hardness < 150
          && (prefab.GetComponent<DrowningMonitor>()?.IsCellSafe(cell) ?? true)
          && (prefab.GetComponent<EntombVulnerable>()?.IsCellSafe(cell) ?? true)
          && (prefab.GetComponent<OccupyArea>()?.CanOccupyArea(cell, ObjectLayer.Building) ?? true)
          //&& (prefab.GetComponent<PressureVulnerable>()?.IsNormal ?? true)
          //&& (prefab.GetComponent<TemperatureVulnerable>()?.IsCellSafe(cell) ?? true)
          && (prefab.GetComponent<UprootedMonitor>()?.IsCellSafe(cell) ?? true);
      }
    }
  }
}