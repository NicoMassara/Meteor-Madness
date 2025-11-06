using _Main.Scripts.Save;
using UnityEngine;
using UnityEditor;

namespace _Main.Scripts.InspectorTools
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
    public class SaveFileTester : MonoBehaviour
    {
        [SerializeField] private string saveFileName;
        [SerializeField] private TestSaveData saveData;

        public void Save()
        {
            SaveSystem.Save(saveData,saveFileName);
        }
        
        public void Load()
        {
            var temp = SaveSystem.LoadSaveFile<TestSaveData>(saveFileName);
            if(temp == null) return;
            Debug.Log($"Score: {temp.Score}\n" +
                      $"HighScore: {temp.HighScore}\n" +
                      $"PlayerName: {temp.PlayerName}");
        }

        public void Delete()
        {
            SaveSystem.DeleteSaveFile(saveFileName);
        }

        public void DeleteAll()
        {
            SaveSystem.DeleteAllSaves();
        }
    }

    [System.Serializable]
    public class TestSaveData : SaveDataBase
    {
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
            if (GUILayout.Button("Delete"))
            {
                script.Delete();
            }
            if (GUILayout.Button("Delete All"))
            {
                script.DeleteAll();
            }
        }
    }
#endif
}