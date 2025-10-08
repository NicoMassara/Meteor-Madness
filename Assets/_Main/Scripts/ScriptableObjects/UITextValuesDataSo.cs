using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_UITextValuesData_Name", menuName = "Scriptable Objects/Game UI Config/Text Data", order = 0)]
    public class UITextValuesDataSo : ScriptableObject, IUITextData
    {
        [SerializeField] private string points = "Score";
        [SerializeField] private string deathPoints = "Your Score";
        [SerializeField] private string deathText = "Humanity is Over";
        [SerializeField] private string gameCountdownText = "Prepare In";
        [SerializeField] private string gameCountdownFinish = "Defend!";

        public string Points => points;
        public string DeathPoints => deathPoints;
        public string DeathText => deathText;
        public string GameCountdownText => gameCountdownText;
        public string GameCountdownFinish => gameCountdownFinish;
    }
}