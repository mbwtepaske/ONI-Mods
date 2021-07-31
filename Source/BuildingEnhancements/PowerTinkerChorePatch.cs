using HarmonyLib;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(Tinkerable), nameof(Tinkerable.MakePowerTinkerable))]
  public static class PowerTinkerChorePatch
  {
    public static void Postfix(Tinkerable __result) => __result.workTime /= 4;
  }
}
