#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using _Main.Scripts.MyComponents;

namespace _Main.Scripts.DebugGUI
{
    public class DebugGUIManager : SingletonBehaviour<DebugGUIManager>
    {
        public float indentPerLevel = 15f;
        public float windowPadding = 10f;
        public float groupBackgroundAlpha = 0.2f;

        private bool _doesShowGUI = false;
        private Vector2 _scrollPos = Vector2.zero;
        private Rect _windowRect = new Rect(50, 50, 450, 350);

        private List<DebugGroup> _groups = new List<DebugGroup>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _doesShowGUI = !_doesShowGUI;
        }

        public DebugGroup CreateGroup(string groupName, int sortingOrder = 10)
        {
            var group = _groups.Find(g => g.Name == groupName);
            if (group == null)
            {
                group = new DebugGroup { Name = groupName, SortingOrder = sortingOrder };
                _groups.Add(group);
                _groups = _groups.OrderBy(g => g.SortingOrder).ThenBy(g => g.Name).ToList();
            }
            return group;
        }

        private void OnGUI()
        {
            if (!_doesShowGUI) return;

            int fontSize = Mathf.RoundToInt(Screen.height * 0.02f);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = fontSize,
                normal = { textColor = Color.white }
            };

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = fontSize,
                normal = { textColor = Color.white }
            };

            GUIStyle groupBackgroundStyle = new GUIStyle(GUI.skin.box);
            groupBackgroundStyle.normal.background = Texture2D.whiteTexture;

            _windowRect = GUI.Window(12345, _windowRect, windowID =>
            {
                DrawWindowContents(windowID, buttonStyle, labelStyle, groupBackgroundStyle);
            }, "Debug GUI");
        }

        private void DrawWindowContents(int windowID, GUIStyle buttonStyle, GUIStyle labelStyle, GUIStyle groupBackground)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.BeginVertical();
            GUILayout.Space(windowPadding);

            foreach (var group in _groups)
            {
                DrawGroupGUI(group, 0, buttonStyle, labelStyle, groupBackground);
            }

            GUILayout.Space(windowPadding);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 20));
        }

        private void DrawGroupGUI(DebugGroup group, int indentLevel, GUIStyle buttonStyle, GUIStyle labelStyle, GUIStyle groupBackground)
        {
            float indent = indentPerLevel * indentLevel;

            // Botón principal
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            if (GUILayout.Button((group.IsCollapsed ? "+ " : "- ") + group.Name, buttonStyle))
                group.IsCollapsed = !group.IsCollapsed;
            GUILayout.EndHorizontal();

            // Contenido
            if (!group.IsCollapsed)
            {
                GUILayout.BeginVertical();

                // Fondo solo del bloque (no tapa texts)
                var prevColor = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, groupBackgroundAlpha);
                Rect backgroundRect = GUILayoutUtility.GetRect(_windowRect.width, 0);
                GUI.Box(new Rect(backgroundRect.x, backgroundRect.y, _windowRect.width - windowPadding * 2, 1), GUIContent.none, groupBackground);
                GUI.color = prevColor;

                foreach (var entry in group.Entries)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(indent + indentPerLevel);
                    GUILayout.Label(entry?.Invoke() ?? "", labelStyle); // SIN fondo
                    GUILayout.EndHorizontal();
                }

                foreach (var subGroup in group.SubGroup)
                {
                    DrawGroupGUI(subGroup, indentLevel + 1, buttonStyle, labelStyle, groupBackground);
                }

                GUILayout.EndVertical();
            }
        }
    }

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
                if (!Entries.Contains(item)) Entries.Add(item);
        }

        public DebugGroup CreateSubGroup(string groupName, int sortingOrder = 10)
        {
            var group = SubGroup.Find(g => g.Name == groupName);
            if (group == null)
            {
                group = new DebugGroup { Name = groupName, SortingOrder = sortingOrder };
                SubGroup.Add(group);
                SubGroup = SubGroup.OrderBy(g => g.SortingOrder).ThenBy(g => g.Name).ToList();
            }
            return group;
        }
    }
}
#endif
