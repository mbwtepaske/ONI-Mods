using HarmonyLib;

using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(LiquidReservoirConfig), nameof(LiquidReservoirConfig.ConfigureBuildingTemplate))]
  public static class LiquidReservoirConfigPatch
  {
    public static void Postfix(GameObject go, Tag prefab_tag) => ReservoirConfigPatch.IncreaseStorage(go);
  }
}