using Harmony;

namespace CritterTuning
{
  [HarmonyPatch(typeof(AtmoSuitConfig), nameof(AtmoSuitConfig.CreateEquipmentDef))]
  public static class AtmoSuitTuning
  {
    public static void Postfix(EquipmentDef __result)
    {
      foreach (var modifier in __result.AttributeModifiers)
      {
        if (modifier.AttributeId == TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS)
        {
          modifier.SetValue(-2);
        }
      }
    }
  }
}
