using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

namespace ConfigurablePipes
{
  public static class ConduitDisplayPortPatches
  {
    [HarmonyPatch(typeof(BuildingCellVisualizer))]
    [HarmonyPatch("DrawIcons")]
    public static class DrawPorts
    {
      // cache variables
      private readonly static HashSet<string> Buildings = new();

      public static bool Prefix(BuildingCellVisualizer __instance, HashedString mode)
      {
        if (Buildings.Contains(__instance.GetBuilding().Def.PrefabID))
        {
          var gameObject = __instance.GetBuilding().gameObject;
          var controller = gameObject.GetComponent<PortDisplayController>();

          return controller == null || controller.Draw(__instance, mode, gameObject);
        }
        return true;
      }

      internal static bool HasBuilding(string name) => Buildings.Contains(name);

      // Add a building to the cache
      internal static void AddBuilding(string ID) => Buildings.Add(ID);
    }

    // Assign cells for ports while building to prevent other buildings from adding ports at the same cells
    [HarmonyPatch(typeof(BuildingDef))]
    [HarmonyPatch("MarkArea")]
    public static class MarkArea
    {
      public static void Postfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
      {
        foreach (var portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
        {
          var secondaryConduitType2 = portDisplay.type;
          var objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
          var rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
          var cell11 = Grid.OffsetCell(cell, rotatedCellOffset8);

          __instance.MarkOverlappingPorts(Grid.Objects[cell11, (int)objectLayerForConduitType4], go);

          Grid.Objects[cell11, (int)objectLayerForConduitType4] = go;
        }
      }
    }

    // Check if ports are blocked prior to building
    [HarmonyPatch(typeof(BuildingDef))]
    [HarmonyPatch("AreConduitPortsInValidPositions")]
    public static class AreConduitPortsInValidPositions
    {
      public static void Postfix(BuildingDef __instance, ref bool __result, GameObject source_go, int cell, Orientation orientation, ref string fail_reason)
      {
        if (__result)
        {
          foreach (var portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
          {
            var rotatedCellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
            var utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);

            __result = (bool)ReadPrivate.Call(__instance, "IsValidConduitConnection", source_go, portDisplay.type, utility_cell, ref fail_reason);

            if (!__result)
            {
              return;
            }
          }
        }
      }
    }
  }
}