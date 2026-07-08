using UnityEngine;

namespace ETEngine.TutorialSystem
{
    public class TutorialSignal //this is hardcode tutorial
    {
        public string message;
        public Vector3 targetPosition;

        public TutorialSignal(string message, Vector3 targetPosition = default(Vector3))
        {
            this.message = message;
            this.targetPosition = targetPosition;
        }
    }
}