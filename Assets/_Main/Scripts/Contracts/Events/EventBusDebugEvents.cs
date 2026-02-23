using System;

namespace _Main.Scripts.Contracts.Events
{
    public class EventBusDebugEvents
    {
        public static event Action<EventBusDebugEntry> OnEntryAdded;
        
        public static void TriggerOnEntryAdded(EventBusDebugEntry entry)
        {
            OnEntryAdded?.Invoke(entry);
        }
    }
    
    public enum EventBusActionType
    {
        Subscribe,
        Publish,
        Unsubscribe
    }

    public struct EventBusDebugEntry
    {
        public int Frame;
        public float Time;
        public string EventName;
        public string Owner;
        public EventBusActionType ActionType;
    }
}