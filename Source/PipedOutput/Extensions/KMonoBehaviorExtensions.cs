using System;

namespace UnityEngine
{
  internal static class KMonoBehaviorExtensions
  {
    internal static void Subscribe(this KMonoBehaviour behavior, GameHashes hash, Action<object> handler)
      => behavior.Subscribe((int)hash, handler);

    internal static int Subscribe<TComponent>(this KMonoBehaviour behaviour, GameHashes hash, EventSystem.IntraObjectHandler<TComponent> handler)
      where TComponent : Component
      => behaviour.Subscribe((int)hash, handler);

    internal static int Subscribe<TComponent>(this KMonoBehaviour behaviour, GameHashes hash, Action<object> handler)
      where TComponent : Component
      => behaviour.Subscribe((int)hash, handler);

    internal static int Subscribe<TComponent>(this KMonoBehaviour behaviour, GameObject target, GameHashes hash, Action<object> handler)
      where TComponent : Component
      => behaviour.Subscribe(target, (int)hash, handler);

    internal static void Trigger(this KMonoBehaviour behavior, int hash, object data = null) => behavior.Trigger(hash, data);
  }
}