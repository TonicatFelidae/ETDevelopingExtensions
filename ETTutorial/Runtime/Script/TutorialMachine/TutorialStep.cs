using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace ETEngine.TutorialSystem
{
    [System.Serializable]
    public struct TutorialStep
    {
        public TutorialTarget target;
        public bool showHighlight;
        public bool showStandout;
        public bool showSpotLight;
        public float spotLightRadius;
        public bool showText;
        public string instructionText;
        public bool showPopup;
        public GameObject pp_popup;
        public bool showOverlay;
        [FormerlySerializedAs("targetOverlay")]
        public GameObject overlay;
        public Vector3 popupOffset;
        public OnTutorialStepComplete onCompleted;
        public UnityEvent onCompletedFeedback;
        public NextStepTriggerType nextStepTriggerType;
        public bool transitionDelay;
        public BackdropType transitionDelayBackdropType;
        public float transitionDelayDuration;
        public bool transitionAfterDelay;
        public BackdropType transitionAfterDelayBackdropType;

    }
}