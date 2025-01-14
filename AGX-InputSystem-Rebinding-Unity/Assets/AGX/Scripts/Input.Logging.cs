using UnityEngine;

namespace AGX.Scripts
{
    public partial class Input
    {
        public void RegisterLogging()
        {
            OnJumpEvent += () => Debug.Log("Jumped");
            OnMoveEvent += () => Debug.Log("Moved");
            OnStartEvent += () => Debug.Log("Started");
        }
    }
}