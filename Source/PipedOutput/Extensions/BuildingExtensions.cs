namespace UnityEngine
{
  internal static class BuildingExtensions
  {
    internal static int GetCellWithOffset(this Building building, CellOffset offset) => Grid.OffsetCell(Grid.PosToCell(building.transform.GetPosition()), building.GetRotatedOffset(offset));
  }
}