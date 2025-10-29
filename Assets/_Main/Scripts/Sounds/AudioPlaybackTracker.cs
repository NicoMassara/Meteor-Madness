using System.Collections.Generic;
using _Main.Scripts.Interfaces;

namespace _Main.Scripts.Sounds
{
    public class AudioPlaybackTracker
    {
        private readonly List<ITrackedAudio> _trackedAudios = new List<ITrackedAudio>();
        private readonly List<ITrackedAudio> _toRemove = new List<ITrackedAudio>();
        private readonly List<ITrackedAudio> _toAdd = new List<ITrackedAudio>();

        public void Execute()
        {
            ApplyPending();

            foreach (var item in _trackedAudios)
            {
                if (item.GetIsPlaying()) continue;
                
                item.TriggerFinish();
                _toRemove.Add(item);
            }
            
            ApplyPending();
        }

        public void Register(ITrackedAudio trackedAudio)
        {
            _toAdd.Add(trackedAudio);
        }

        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                foreach (var item in _toAdd)
                {
                    _trackedAudios.Add(item);
                }
                
                _toAdd.Clear();
            }

            if (_toRemove.Count > 0)
            {
                foreach (var item in _toRemove)
                {
                    _trackedAudios.Remove(item);
                }
                
                _toRemove.Clear();
            }
        }
    }
}