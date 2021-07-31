using HarmonyLib;
using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(AlgaeHabitatConfig), nameof(AlgaeHabitatConfig.ConfigureBuildingTemplate))]
  public static class AlgaeHabitatConfigPatch
  {
    public static void Postfix(GameObject go, Tag prefab_tag)
    {
      if (go.GetComponent<AlgaeHabitat>() is AlgaeHabitat algaeHabitat)
      {
        algaeHabitat.lightBonusMultiplier = 4;
      }
    }
  }
}
