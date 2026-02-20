using System;
using System.Collections.Generic;
using _Main.Scripts.Contracts.Events;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.DebugTools.GUIDebug
{
    public class EventBusDebug : MonoBehaviour
    {
        private class EntryController
        {
            private readonly List<EventBusDebugEntry> _entries = new();
            private readonly int MaxEntries;
            
            public IReadOnlyList<EventBusDebugEntry> Entries => _entries;

            public EntryController(int maxEntries = 300)
            {
                MaxEntries = maxEntries;
                EventBusDebugEvents.OnEntryAdded += OnEntryAddedHandler;
            }
            
            private void OnEntryAddedHandler(EventBusDebugEntry input)
            {
                Log(input);
            }
            
            private void Log(EventBusDebugEntry entryData)
            {
                if (_entries.Count >= MaxEntries)
                    _entries.RemoveAt(_entries.Count - 1);
                
                entryData.Frame = Time.frameCount;
                entryData.Time = Time.time;

                _entries.Insert(0,entryData);
            }
            
            public void Clear() => _entries.Clear();
        }

        private EntryController _entryController;
        private Vector2 _scroll;
        private bool _show = true;
        private Rect _windowRect = new Rect(10, 10, 600, 400);

        private void Awake()
        {
            _entryController = new EntryController();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
                _show = !_show;
        }

        private void OnGUI()
        {
            if (!_show) return;

            _windowRect = GUI.Window(0, _windowRect, DrawWindow, "Event Bus Debugger");
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
                _entryController.Clear();
            GUILayout.EndHorizontal();

            _scroll = GUILayout.BeginScrollView(_scroll);

            foreach (var entry in _entryController.Entries)
                DrawEntry(entry);

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void DrawEntry(EventBusDebugEntry entry)
        {
            var color = Color.green;
            
            switch (entry.ActionType)
            {
                case EventBusActionType.Publish:
                    color = Color.green;

                    if (entry.Frame % 2 == 0)
                    {
                        color.g = 0.75f;
                    }

                    GUI.color = color;
                    GUILayout.Label($"[{entry.Frame}] {entry.ActionType} -> {entry.EventName}");
                    break;

                case EventBusActionType.Subscribe:
                    
                    color = Color.cyan;

                    if (entry.Frame % 2 == 0)
                    {
                        color.b = 0.75f;
                    }

                    GUI.color = color;
                    
                    GUILayout.Label($"[{entry.Frame}] {entry.ActionType} -> {entry.EventName}");
                    break;

                case EventBusActionType.Unsubscribe:
                    
                    color = Color.red;

                    if (entry.Frame % 2 == 0)
                    {
                        color.r = 0.75f;
                    }
                    
                    GUI.color = color;
                    GUILayout.Label($"[{entry.Frame}] {entry.ActionType} -> {entry.EventName}");
                    break;
            }

            GUI.color = Color.white;
        }
        
    }
}