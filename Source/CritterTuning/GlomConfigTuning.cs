using Harmony;
using UnityEngine;

namespace CritterTuning
{
  [HarmonyPatch(typeof(GlomConfig), nameof(GlomConfig.CreatePrefab))]
  public static class GlomConfigTuning
  {
    public static void Postfix(GameObject __result)
    {
      var definition = __result.AddOrGetDef<ElementDropperMonitor.Def>();

      //definition.dirtyProbabilityPercent = 1F;
      definition.dirtyMassPerDirty = 10F;
      definition.emitDiseasePerKg = 0;
    }
  }
}