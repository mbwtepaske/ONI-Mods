using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;

using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(PrimaryElement))]
  [HarmonyPatch(MethodType.StaticConstructor)]
  public static class PrimaryElementPatch
  {
    [HarmonyPostfix]
    public static void IncreaseMaximumMass() => PrimaryElement.MAX_MASS *= 4;

    //[HarmonyTranspiler]
    //public static IEnumerable<CodeInstruction> IncreaseMaximumMass(IEnumerable<CodeInstruction> instructions)
    //{
    //  Debug.LogFormat("PrimaryElementPatch started!");
    //
    //  return instructions;
    //
    //  //var primaryElementType = typeof(PrimaryElement);
    //  //var maximumMassField = primaryElementType.GetField(nameof(PrimaryElement.MAX_MASS));
    //
    //  //foreach (var instruction in instructions)
    //  //{
    //  //  if (instruction.opcode == OpCodes.Stsfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == maximumMassField.Name)
    //  //  {
    //  //    Debug.LogFormat("PrimaryElementPatch started!");
    //
    //  //    yield return new CodeInstruction(OpCodes.Ldc_R4, 4F);
    //  //    yield return new CodeInstruction(OpCodes.Mul);
    //  //  }
    //
    //  //  yield return instruction;
    //  //}
    //}
  }
}