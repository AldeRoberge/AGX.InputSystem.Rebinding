using Sirenix.OdinInspector;
using UnityEngine;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Utilities
{
    public class SetMaxFPS : MonoBehaviour
    {
        [BoxGroup("Data"), SerializeField, MinValue(1)] private int _maxFPS = 120;
        
        void Start()
        {
            Application.targetFrameRate = _maxFPS; // Set to 60 FPS (or higher)
            QualitySettings.vSyncCount = 0; // Disable V-Sync to allow Unity to control FPS
        }
    }
}