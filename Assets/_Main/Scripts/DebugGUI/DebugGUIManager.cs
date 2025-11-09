using System;
using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using UnityEngine;

namespace _Main.Scripts.DebugGUI
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Serializable]
    public class DebugGroup
    {
        public string Name;
        public bool IsCollapsed = false;
        public List<Func<string>> Entries = new List<Func<string>>();
        private List<Func<string>> _pendingToAdd = new List<Func<string>>();
        private List<Func<string>> _pendingToRemove = new List<Func<string>>();

        public void ApplyPending()
        {
            if (_pendingToAdd.Count > 0)
            {
                foreach (var entry in _pendingToAdd)
                {
                    Entries.Add(entry);
                }
                
                _pendingToAdd.Clear();
            }

            if (_pendingToRemove.Count > 0)
            {
                foreach (var entry in _pendingToRemove)
                {
                    Entries.Remove(entry);
                }
                
                _pendingToRemove.Clear();
            }
        }

        public void AddEntry(params Func<string>[] entry)
        {
            foreach (var item in entry)
            {
                if(Entries.Contains(item)) continue;
            
                _pendingToAdd.Add(item);
            }
        }
        
        public void AddEntry(Func<string> entry)
        {
            if(Entries.Contains(entry)) return;
            
            _pendingToAdd.Add(entry);
        }

        public void RemoveEntry(Func<string> entry)
        {
            if(!Entries.Contains(entry)) return;
            
            _pendingToRemove.Remove(entry);
        }
    }

    public class DebugGUIManager : SingletonBehaviour<DebugGUIManager>
    {
        public Vector2 startPos = new Vector2(10, 10);
        public float lineHeight = 20f;
        public float buttonHeight = 25f;
        public float groupScaling = 10f;
        
        private readonly List<DebugGroup> _groups = new List<DebugGroup>();


        public DebugGroup CreateGroup(string groupName)
        {
            var group = _groups.Find(g=>g.Name == groupName);
            if (group == null)
            {
                group = new DebugGroup
                {
                    Name = groupName
                };
                _groups.Add(group);
            }
            
            return group;
        }
        
        private void OnGUI()
        {
            Vector2 pos = startPos;

            foreach (var group in _groups)
            {
                string buttonText = group.Name;
                
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    normal =
                    {
                        textColor = Color.white
                    }
                };
                
                if (GUI.Button(new Rect(pos.x, pos.y, 100, buttonHeight), 
                        (group.IsCollapsed ? $"+ {buttonText}" : $"- {buttonText}"), buttonStyle))
                {
                    group.IsCollapsed = !group.IsCollapsed;
                }

                pos.y += buttonHeight + 2;

                if (!group.IsCollapsed)
                {

                    group.ApplyPending();
                    
                    foreach (var entry in group.Entries)
                    {
                        GUI.Label(new Rect(pos.x,pos.y,500,lineHeight), entry?.Invoke());
                        pos.y += lineHeight;
                    }
                    
                    group.ApplyPending();
                }
                
                pos.y += groupScaling;
            }
        }
    }
    
#endif

}