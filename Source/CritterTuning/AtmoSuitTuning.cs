using HarmonyLib;
using TUNING;

namespace CritterTuning
{
  [HarmonyPatch(typeof(AtmoSuitConfig), nameof(AtmoSuitConfig.CreateEquipmentDef))]
  public static class AtmoSuitTuning
  {
    public static void Postfix(EquipmentDef __result)
    {
      foreach (var modifier in __result.AttributeModifiers)
      {
        if (modifier.AttributeId == EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS)
        {
          modifier.SetValue(-2);
        }
      }
    }
  }
}
