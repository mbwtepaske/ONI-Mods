using Harmony;
using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(RefrigeratorConfig), nameof(RefrigeratorConfig.DoPostConfigureComplete))]
  public static class RefrigeratorConfigPatch
  {
    public static void Postfix(GameObject go)
    {
      if (go.GetComponent<Storage>() is Storage storage)
      {
        storage.capacityKg *= 8;
      }
    }
  }
}
