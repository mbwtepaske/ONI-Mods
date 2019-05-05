﻿using Harmony;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(ModifierSet), nameof(ModifierSet.Initialize))]
  public static class PowerTinkerEffectPatch
  {
    public static void Postfix(ModifierSet __instance)
    {
      var powerTinkerEffect = __instance.effects.TryGet("PowerTinker");

      if (powerTinkerEffect != null)
      {
        powerTinkerEffect.duration *= 4;
      }

      //File.AppendAllText(@"E:\Steam\steamapps\common\OxygenNotIncluded\Mods\Test.log", $"PowerTinkerEffectPatch = {powerTinkerEffect?.duration ?? -1}");
    }
  }
}
