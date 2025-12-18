using System.Collections;
using _Main.Scripts.GlobalEvents;
using Unity.Services.Core;
using UnityEngine;

namespace _Main.Scripts
{
    public class UGSInitializer : MonoBehaviour
    {
        private void Awake()
        {
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            StartCoroutine(Coroutine_Initialize());
        }
        
        private IEnumerator Coroutine_Initialize()
        {
            var initTask = UnityServices.InitializeAsync();

            while (!initTask.IsCompleted)
                yield return null;

            if (initTask.IsFaulted)
                Debug.LogError("Couldn't Initialize UGS: " + initTask.Exception);
            else
            {
                BootEvents.MainSystemInitialized();
            }
        }
    }
}