using HarmonyLib;

using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(GasReservoirConfig), nameof(GasReservoirConfig.ConfigureBuildingTemplate))]
  public static class GasReservoirConfigPatch
  {
    public static void Postfix(GameObject go, Tag prefab_tag) => ReservoirConfigPatch.IncreaseStorage(go);
  }
}
