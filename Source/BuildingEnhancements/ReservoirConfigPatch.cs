using UnityEngine;

namespace BuildingEnhancements
{
  public static class ReservoirConfigPatch
  {
    public static void IncreaseStorage(GameObject go)
    {
      if (go.GetComponent<Storage>() is Storage storage && go.GetComponent<ConduitConsumer>() is ConduitConsumer consumer)
      {
        storage.capacityKg  *= 8;
        consumer.capacityKG =  storage.capacityKg;
      }
    }
  }
}