using System;
using System.IO;
using System.Linq;

using Harmony;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(PlantableSeed), nameof(PlantableSeed.Sim200ms))]
  public static class PlantableSeedTimingPatch
  {
    [HarmonyPrefix]
    public static void Fix(PlantableSeed __instance)
    {
      if (__instance.timeUntilSelfPlant > 60)
      {
        __instance.timeUntilSelfPlant = 60;
      }
    }
  }

  [HarmonyPatch(typeof(PlantableSeed), "TestSuitableGround")]
  public static class PlantableSeedTestingPatch
  {
    [HarmonyPostfix]
    public static void Fix(PlantableSeed __instance, int cell, ref bool __result)
    {
      var prefab = Assets.GetPrefab(__instance.PlantID);
      
      if (!__result)
      {
        __result = Grid.IsValidCell(cell) && !Grid.Foundation[Grid.CellBelow(cell)]
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