using UnityEngine;

namespace ETEngine.TutorialSystem
{
    public abstract class TutorialTarget : MonoBehaviour
    {

        public abstract void HighlightTarget();
        public abstract void StandOutTarget();
        public abstract void SpotlightTarget(float radius);
        public abstract void StepFeedback();
        public abstract void DisableTutorial();
    }
}