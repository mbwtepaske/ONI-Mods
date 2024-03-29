﻿using System;

using STRINGS;
using TUNING;
using UnityEngine;

namespace ConfigurablePipes
{
  [SkipSaveFileSerialization]
  internal class PortConduitConsumer : KMonoBehaviour
  {
    internal enum WrongElementResult
    {
      Destroy,
      Dump,
      Store
    }

    [SerializeField]
    internal CellOffset conduitOffset;

    [SerializeField]
    internal CellOffset conduitOffsetFlipped;

    [SerializeField]
    internal ConduitType conduitType;

    [SerializeField]
    internal bool ignoreMinMassCheck;

    [SerializeField]
    internal Tag capacityTag = GameTags.Any;

    [SerializeField]
    internal float capacityKG = float.PositiveInfinity;

    [SerializeField]
    internal bool forceAlwaysSatisfied;

    [SerializeField]
    internal bool alwaysConsume;

    [SerializeField]
    internal bool keepZeroMassObject = true;

    [NonSerialized]
    internal bool isConsuming = true;

    private FlowUtilityNetwork.NetworkItem networkItem;

    [MyCmpReq]
    readonly private Operational operational;

    [MyCmpReq]
    readonly private Building building;

    [MyCmpGet]
    internal Storage storage;

    private int utilityCell = -1;

    internal float consumptionRate = float.PositiveInfinity;

    internal static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);

    private HandleVector<int>.Handle partitionerEntry;

    private bool satisfied;

    internal ConduitConsumer.WrongElementResult wrongElementResult;

    internal void AssignPort(PortDisplayInput port)
    {
      this.conduitType = port.type;
      this.conduitOffset = port.offset;
      this.conduitOffsetFlipped = port.offsetFlipped;
    }

    internal bool IsConnected
    {
      get
      {
        var gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
        return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
      }
    }

    internal bool CanConsume
    {
      get
      {
        var result = false;
        if (this.IsConnected)
        {
          var conduitManager = this.GetConduitManager();
          result = (conduitManager.GetContents(this.utilityCell).mass > 0f);
        }
        return result;
      }
    }

    internal ConduitType TypeOfConduit
    {
      get
      {
        return this.conduitType;
      }
    }

    internal bool IsAlmostEmpty
    {
      get
      {
        return !this.ignoreMinMassCheck && this.MassAvailable < this.ConsumptionRate * 30f;
      }
    }

    internal bool IsEmpty
    {
      get
      {
        return !this.ignoreMinMassCheck && (this.MassAvailable == 0f || this.MassAvailable < this.ConsumptionRate);
      }
    }

    internal float ConsumptionRate
    {
      get
      {
        return this.consumptionRate;
      }
    }

    internal bool IsSatisfied
    {
      get
      {
        return this.satisfied || !this.isConsuming;
      }
      set
      {
        this.satisfied = (value || this.forceAlwaysSatisfied);
      }
    }

    internal float MassAvailable
    {
      get
      {
        var inputCell = this.GetInputCell();
        var conduitManager = this.GetConduitManager();
        return conduitManager.GetContents(inputCell).mass;
      }
    }

    internal void SetConduitData(ConduitType type)
    {
      this.conduitType = type;
    }

    private ConduitFlow GetConduitManager()
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

    private int GetInputCell()
    {
      var building = GetComponent<Building>();
      return building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.conduitOffset : this.conduitOffsetFlipped);
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();
      this.utilityCell = this.GetInputCell();

      var networkManager = Conduit.GetNetworkManager(this.conduitType);
      this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Sink, this.utilityCell, gameObject);
      networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

      var layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
      this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
      this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
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

    private void OnConduitConnectionChanged(object data)
    {
      Trigger(-2094018600, this.IsConnected);
    }

    private void ConduitUpdate(float dt)
    {
      if (this.isConsuming)
      {
        var conduitManager = this.GetConduitManager();
        this.Consume(dt, conduitManager);
      }
    }

    private void Consume(float dt, ConduitFlow conduit_mgr)
    {
      if (this.building.Def.CanMove)
      {
        this.utilityCell = this.GetInputCell();
      }
      if (this.IsConnected)
      {
        var contents = conduit_mgr.GetContents(this.utilityCell);
        if (contents.mass > 0f)
        {
          this.IsSatisfied = true;
          if (this.alwaysConsume || this.operational.IsOperational)
          {
            var num = (!(this.capacityTag != GameTags.Any)) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag);
            var b = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
            var num2 = this.ConsumptionRate * dt;
            num2 = Mathf.Min(num2, b);
            var num3 = 0f;
            if (num2 > 0f)
            {
              num3 = conduit_mgr.RemoveElement(this.utilityCell, num2).mass;
            }
            var element = ElementLoader.FindElementByHash(contents.element);
            var flag = element.HasTag(this.capacityTag);
            if (num3 > 0f && this.capacityTag != GameTags.Any && !flag)
            {
              Trigger(-794517298, new BuildingHP.DamageSourceInfo
              {
                damage = 1,
                source = STRINGS.BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
                popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
              });
            }
            if (flag || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any)
            {
              if (num3 > 0f)
              {
                var disease_count = (int)((float)contents.diseaseCount * (num3 / contents.mass));
                var element2 = ElementLoader.FindElementByHash(contents.element);
                var conduitType = this.conduitType;
                if (conduitType != ConduitType.Liquid)
                {
                  if (conduitType == ConduitType.Gas)
                  {
                    if (element2.IsGas)
                    {
                      this.storage.AddGasChunk(contents.element, num3, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
                    }
                    else
                    {
                      global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString());
                    }
                  }
                }
                else if (element2.IsLiquid)
                {
                  this.storage.AddLiquid(contents.element, num3, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
                }
                else
                {
                  global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString());
                }
              }
            }
            else if (num3 > 0f && this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
            {
              var disease_count2 = (int)((float)contents.diseaseCount * (num3 / contents.mass));
              var gameCell = Grid.PosToCell(transform.GetPosition());
              SimMessages.AddRemoveSubstance(gameCell, contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num3, contents.temperature, contents.diseaseIdx, disease_count2, true, -1);
            }
          }
        }
        else
        {
          this.IsSatisfied = false;
        }
      }
      else
      {
        this.IsSatisfied = false;
      }
    }
  }
}