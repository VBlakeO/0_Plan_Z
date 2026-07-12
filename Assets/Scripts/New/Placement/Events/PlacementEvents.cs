using UnityEngine;
using PlanZ.Events;
using PlanZ.Placement.Data;

namespace PlanZ.Placement.Events
{
    public readonly struct PlacementEnteredEvent : IEvent
    {
        public PlaceableData Data { get; }
        public PlacementEnteredEvent(PlaceableData data) => Data = data;
    }

    public readonly struct PlacementExitedEvent : IEvent
    {
        public PlaceableData Data { get; }
        public PlacementExitedEvent(PlaceableData data) => Data = data;
    }

    public readonly struct PlacementPlacedEvent : IEvent
    {
        public PlaceableData Data { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public PlacementPlacedEvent(PlaceableData data, Vector3 position, Quaternion rotation)
        {
            Data = data;
            Position = position;
            Rotation = rotation;
        }
    }

    public readonly struct PlacementCancelledEvent : IEvent
    {
        public PlaceableData Data { get; }
        public PlacementCancelledEvent(PlaceableData data) => Data = data;
    }

    public readonly struct PlaceableInventoryChangedEvent : IEvent
    {
        public PlaceableData Data { get; }
        public int Amount { get; }

        public PlaceableInventoryChangedEvent(PlaceableData data, int amount)
        {
            Data = data;
            Amount = amount;
        }
    }
}
