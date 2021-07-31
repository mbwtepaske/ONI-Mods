using System;
using System.Collections.Generic;

using KSerialization;

using UnityEngine;

namespace ConfigurablePipes
{
  [SerializationConfig(MemberSerialization.OptIn)]
  internal abstract class PortConduitDispenserBase : KMonoBehaviour, ISaveLoadable
  {
    [SerializeField]
    public CellOffset conduitOffset;

    [SerializeField]
    public CellOffset conduitOffsetFlipped;

    [SerializeField]
    public ConduitType conduitType;

    [SerializeField]
    public SimHashes[] elementFilter = null;

    [SerializeField]
    public bool invertElementFilter;

    [SerializeField]
    public bool alwaysDispense;

    [SerializeField]
    public bool SkipSetOperational = false;

    private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

    private FlowUtilityNetwork.NetworkItem networkItem;

    [MyCmpReq]
    readonly private Operational operational;

    [MyCmpReq]
    public Storage storage;

    private HandleVector<int>.Handle partitionerEntry;

    private int utilityCell = -1;

    private int elementOutputOffset;

    internal void AssignPort(PortDisplayOutput port)
    {
      this.conduitType          = port.type;
      this.conduitOffset        = port.offset;
      this.conduitOffsetFlipped = port.offsetFlipped;
    }

    internal ConduitType TypeOfConduit
    {
      get
      {
        return this.conduitType;
      }
    }

    internal ConduitFlow.ConduitContents ConduitContents
    {
      get
      {
        return this.GetConduitManager().GetContents(this.utilityCell);
      }
    }

    public int UtilityCell
    {
      get
      {
        return this.utilityCell;
      }
    }

    internal bool IsConnected
    {
      get
      {
        var gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
        return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
      }
    }

    internal void SetConduitData(ConduitType type)
    {
      this.conduitType = type;
    }

    internal ConduitFlow GetConduitManager()
    {
      var conduitType = this.conduitType;
      if (conduitType == ConduitType.Gas)
      {
        return Game.Instance.gasConduitFlow;
      }
      if (conduitType != ConduitType.Liquid)
      {
        return null;
      }
      return Game.Instance.liquidConduitFlow;
    }

    private void OnConduitConnectionChanged(object data)
    {
      Trigger(-2094018600, this.IsConnected);
    }

    internal virtual CellOffset GetUtilityCellOffset()
    {
      return new CellOffset(0, 1);
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();

      var building = GetComponent<Building>();
      this.utilityCell = building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.conduitOffset : this.conduitOffsetFlipped);
      var networkManager = Conduit.GetNetworkManager(this.conduitType);
      this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.utilityCell, gameObject);
      networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

      var layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
      this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
      this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.LastPostUpdate);
      this.OnConduitConnectionChanged(null);
    }

    protected override void OnCleanUp()
    {
      var networkManager = Conduit.GetNetworkManager(this.conduitType);
      networkManager.RemoveFromNetworks(this.utilityCell, this.networkItem, true);

      this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
      GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
      base.OnCleanUp();
    }

    protected virtual void ConduitUpdate(float dt)
    {
      if (!SkipSetOperational)
      {
        this.operational.SetFlag(outputConduitFlag, this.IsConnected);
      }
      if (this.operational.IsOperational || this.alwaysDispense)
      {
        var primaryElement = this.FindSuitableElement();
        if (primaryElement != null)
        {
          primaryElement.KeepZeroMassObject = true;
          var conduitManager = this.GetConduitManager();
          var       num            = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
          if (num > 0f)
          {
            var num2 = num / primaryElement.Mass;
            var   num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
            primaryElement.ModifyDiseaseCount(-num3, "CustomConduitDispenser.ConduitUpdate");
            primaryElement.Mass -= num;
            Trigger(-1697596308, primaryElement.gameObject);
          }
        }
      }
    }

    protected virtual PrimaryElement FindSuitableElement()
    {
      var items = this.storage.items;
      var              count = items.Count;
      for (var i = 0; i < count; i++)
      {
        var            index     = (i + this.elementOutputOffset) % count;
        var component = items[index].GetComponent<PrimaryElement>();
        if (component != null && component.Mass > 0f && ((this.conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
        {
          this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
          return component;
        }
      }
      return null;
    }

    private bool IsFilteredElement(SimHashes element)
    {
      for (var num = 0; num != this.elementFilter.Length; num++)
      {
        if (this.elementFilter[num] == element)
        {
          return true;
        }
      }
      return false;
    }
  }
}