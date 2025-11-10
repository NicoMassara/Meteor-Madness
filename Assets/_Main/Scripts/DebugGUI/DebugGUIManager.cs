using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.MyComponents;
using UnityEngine;

namespace _Main.Scripts.DebugGUI
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Serializable]
    public class DebugGroup
    {
        public string Name;
        public int SortingOrder = 10;
        public bool IsCollapsed = true;
        public List<Func<string>> Entries = new List<Func<string>>();
        public List<DebugGroup> SubGroup = new List<DebugGroup>();

        public void AddEntry(params Func<string>[] entry)
        {
            foreach (var item in entry)
            {
                if(Entries.Contains(item)) continue;
            
                Entries.Add(item);
            }
        }
        public void RemoveEntry(Func<string> entry)
        {
            if(!Entries.Contains(entry)) return;
            
            Entries.Remove(entry);
        }

        public DebugGroup CreateSubGroup(string groupName, int sortingOrder = 10)
        {
            var group = SubGroup.Find(g=>g.Name == groupName);
            if (group == null)
            {
                group = new DebugGroup
                {
                    Name = groupName,
                    SortingOrder = sortingOrder
                };
                
                SubGroup.Add(group);
                
                SubGroup = SubGroup
                    .OrderBy(g => g.SortingOrder)
                    .ThenBy(g => g.Name)
                    .ToList();
            }
            
            return group;
        }

    }

    public class DebugGUIManager : SingletonBehaviour<DebugGUIManager>
    {
        public Vector2 startPos = new Vector2(10, 10);
        public float lineHeight = 20f;
        public float buttonHeight = 25f;
        public float groupScaling = 10f;
        public float indentPerLevel = 15f;
        //Padding
        public float buttonPadding = 15f;
        
        private List<DebugGroup> _groups = new List<DebugGroup>();

        private bool _doesShowGUI = true;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _doesShowGUI = !_doesShowGUI;
            }
        }

        public DebugGroup CreateGroup(string groupName, int sortingOrder = 10)
        {
            var group = _groups.Find(g=>g.Name == groupName);
            if (group == null)
            {
                group = new DebugGroup
                {
                    Name = groupName,
                    SortingOrder = sortingOrder
                };
                
                _groups.Add(group);
                
                _groups = _groups
                    .OrderBy(g => g.SortingOrder)
                    .ThenBy(g => g.Name)
                    .ToList();
            }
            
            return group;
        }


        private void DrawGroups(DebugGroup group, ref Vector2 pos, int indentLevel, GUIStyle buttonStyle, float maxWidth)
        {
            string buttonText = group.Name;
                
            float indent = indentPerLevel * indentLevel;
            
            if (GUI.Button(new Rect(pos.x, pos.y, maxWidth, buttonHeight), 
                    (group.IsCollapsed ? $"+ {buttonText}" : $"- {buttonText}"), buttonStyle))
            {
                group.IsCollapsed = !group.IsCollapsed;
            }
            

            pos.y += buttonHeight + 2;

            if (!group.IsCollapsed)
            {
                var subMaxWidht = 0f;
                SetMaxWidth(group.SubGroup, buttonStyle, ref subMaxWidht);
                
                var guiPosX = pos.x + indent + indentPerLevel;
                
                var boxWidth = 0f;
                SetBoxWidth(group.Entries, buttonStyle, ref boxWidth);
                
                foreach (var entry in group.Entries)
                {
                    // Guardar color original
                    Color prevColor = GUI.color;

                    // Dibujar fondo semitransparente detrás del entry
                    GUI.color = new Color(0f, 0f, 0f, 1);
                    GUI.Box(new Rect(guiPosX - 5, pos.y - 2, boxWidth, lineHeight+5), GUIContent.none);
                    
                    GUI.color = prevColor;
                    GUI.Label(new Rect(guiPosX,pos.y,boxWidth,lineHeight), entry?.Invoke());
                    pos.y += lineHeight;
                }

                foreach (var subGroup in group.SubGroup)
                {
                    DrawGroups(subGroup, ref pos, indentLevel + 1, buttonStyle, subMaxWidht);
                }
            }
                
            pos.y += groupScaling;
        }

        private void SetMaxWidth(List<DebugGroup> groups, GUIStyle buttonStyle, ref float maxWidth)
        {
            foreach (var item in groups)
            {
                Vector2 size = buttonStyle.CalcSize(new GUIContent(item.Name));
                if (size.x > maxWidth)
                    maxWidth = size.x;
            }

            maxWidth += buttonPadding;
        }

        private void SetBoxWidth(List<Func<string>> entries, GUIStyle buttonStyle, ref float boxWidth)
        {
            foreach (var item in entries)
            {
                Vector2 size = buttonStyle.CalcSize(new GUIContent(item?.Invoke()));
                if (size.x > boxWidth)
                    boxWidth = size.x;
            }
        }

        private void OnGUI()
        {
            if(_doesShowGUI == false) return;
            
            Vector2 pos = startPos;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                normal =
                {
                    textColor = Color.white
                }
            };
            
            float maxWidth = 0;
            SetMaxWidth(_groups, buttonStyle, ref maxWidth);
            
            foreach (var group in _groups)
            {
                DrawGroups(group, ref pos,0, buttonStyle,maxWidth);
            }
        }
    }
    
#endif

}