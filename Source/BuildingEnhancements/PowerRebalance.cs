using Harmony;
using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(GasPumpConfig), nameof(GasPumpConfig.CreateBuildingDef))]
  public static class GasPumpConfigPatch
  {
    public static void Postfix(BuildingDef __result) => __result.EnergyConsumptionWhenActive *= 0.5F;
  }

  [HarmonyPatch(typeof(GasMiniPumpConfig), nameof(GasMiniPumpConfig.CreateBuildingDef))]
  public static class GasMiniPumpConfigPatch
  {
    public static void Postfix(BuildingDef __result) => __result.EnergyConsumptionWhenActive *= 0.5F;
  }

  [HarmonyPatch(typeof(GasFilterConfig), nameof(GasFilterConfig.CreateBuildingDef))]
  public static class GasFilterConfigPatch
  {
    public static void Postfix(BuildingDef __result) => __result.EnergyConsumptionWhenActive *= 0.5F;
  }
}
