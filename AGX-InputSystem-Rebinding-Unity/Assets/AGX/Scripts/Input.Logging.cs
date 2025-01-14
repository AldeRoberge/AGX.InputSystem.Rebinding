using UnityEngine;

namespace AGX.Scripts
{
    public partial class Input
    {
        public void RegisterLogging()
        {
            OnJumpEvent += () => Debug.Log("[InputLogging] <b>Jump</b> event was successfully invoked.");
            OnMoveEvent += () => Debug.Log("[InputLogging] <b>Move</b> event was successfully invoked.");
            OnStartEvent += () => Debug.Log("[InputLogging] <b>Start</b> event was successfully invoked.");
        }
    }
}