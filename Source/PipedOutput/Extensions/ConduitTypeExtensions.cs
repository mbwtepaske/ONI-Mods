namespace UnityEngine
{
  internal static class ConduitTypeExtensions
  {
    internal static int GetConduitObjectLayer(this ConduitType conduitType) => conduitType switch
    {
      ConduitType.Gas => (int)ObjectLayer.GasConduit,
      ConduitType.Liquid => (int)ObjectLayer.LiquidConduit,
      ConduitType.Solid => (int)ObjectLayer.SolidConduit,
      _ => 0
    };

    internal static int GetPortObjectLayer(this ConduitType conduitType) => conduitType switch
    {
      ConduitType.Gas => (int)ObjectLayer.GasConduitConnection,
      ConduitType.Liquid => (int)ObjectLayer.LiquidConduitConnection,
      ConduitType.Solid => (int)ObjectLayer.SolidConduitConnection,
      _ => 0
    };

    internal static bool IsConnected(this ConduitType conduitType, int cell) => Grid.Objects[cell, conduitType.GetConduitObjectLayer()] != null;
  }
}