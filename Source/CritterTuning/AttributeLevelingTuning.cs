using Harmony;
using TUNING;

namespace CritterTuning
{
  [HarmonyPatch(typeof(DUPLICANTSTATS.ATTRIBUTE_LEVELING))]
  [HarmonyPatch(MethodType.StaticConstructor)]
  public static class AttributeLevelingTuning
  {
    [HarmonyPostfix]
    public static void BoostMaximumLevel()
    {
      DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL *= 10;

      Debug.Log($"Boosted maximum attribute level: {DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL}");
    }
  }
}