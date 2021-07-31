using HarmonyLib;
using UnityEngine;

namespace BuildingEnhancements
{
  [HarmonyPatch(typeof(StorageLockerConfig), nameof(StorageLockerConfig.DoPostConfigureComplete))]
  public static class StorageLockerConfigPatch
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
