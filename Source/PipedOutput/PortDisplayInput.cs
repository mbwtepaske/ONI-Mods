using UnityEngine;

namespace ConfigurablePipes
{
  internal class PortDisplayInput : DisplayConduitPortInfo
  {
    public PortDisplayInput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, true, color) { }
  }
}