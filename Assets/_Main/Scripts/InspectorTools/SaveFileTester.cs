using _Main.Scripts.Save;
using UnityEngine;
using UnityEditor;

namespace _Main.Scripts.InspectorTools
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
    public class SaveFileTester : MonoBehaviour
    {
        [SerializeField] private TestSaveData saveData;
        [SerializeField] private TestSaveData loadedSaveData;
        [SerializeField] private SaveDataType saveDataToClear;
        
        public void Save()
        {
            DataManager.Instance.SaveGameData(new TestSaveData
            {
                Score = saveData.Score,
                HighScore = saveData.HighScore,
                PlayerName = saveData.PlayerName,
                
            }, saveData.Type);
        }
        
        public void Load()
        {
            loadedSaveData = DataManager.Instance.GetData<TestSaveData>(SaveDataType.Test);
        }

        public void Clear()
        {
            loadedSaveData = null;
            DataManager.Instance.ClearSaveData<TestSaveData>(saveDataToClear);
        }
    }

    [System.Serializable]
    public class TestSaveData : SaveDataBase
    {
        public override SaveDataType Type => SaveDataType.Test;
        public int Score;
        public int HighScore;
        public string PlayerName;
    }
    
#endif
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(SaveFileTester))]
    public class SaveFileTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            SaveFileTester script = (SaveFileTester)target;
            if (GUILayout.Button("Save"))
            {
                script.Save();
            }
            if (GUILayout.Button("Load"))
            {
                script.Load();
            }
            if (GUILayout.Button("Clear"))
            {
                script.Clear();
            }
        }
    }
#endif
}