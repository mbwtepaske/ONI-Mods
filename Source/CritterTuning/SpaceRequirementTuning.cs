using Harmony;

namespace CritterTuning
{
  [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToWildCreature))]
  public static class SpaceRequirementTuning
  {
    public static void Prefix(ref int space_required_per_creature) => space_required_per_creature /= 4;
  }
}
