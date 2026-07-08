using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ETEngine.TutorialSystem
{
    /// <summary>
    /// The machine should already in screen 
    /// </summary>
    public class TutorialMachine : MonoBehaviour
    {
        [SerializeField] private TutorialStep[] tutorialSteps;
        [SerializeField] private bool autoFindAllTargetsOnInit = true;
        [SerializeField] private TutorialTarget[] tutorialTargets;
        private bool _isIgnoreTutorialFeedback = false;
        private bool _isTutorialCompleted = false;
        private int _currentStepIndex = -1;
        private GameObject _activePopup;
        private NextStepTriggerType _nextStepTriggerType;
        private Coroutine _transitionCoroutine;
        public UnityEvent onTutorialStepComplete;
        public UnityEvent onTutorialCompleted;
        public void Init() => Init(true, false, false);
        private bool TutorialDataInvalid => tutorialSteps == null || tutorialSteps.Length == 0;
        private bool TutorialStepOutOfBounds => _currentStepIndex >= tutorialSteps.Length;
        public void Init(bool isFirstTime, bool skipTutorial, bool ignoreTutorialFeedback = false)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }
            _isIgnoreTutorialFeedback = ignoreTutorialFeedback;
            CleanUpCurrentStepListeners();
            if (autoFindAllTargetsOnInit)
            {
                tutorialTargets = GetComponentsInChildren<TutorialTarget>(true);
            }

            if (!TutorialDataInvalid && isFirstTime && !skipTutorial)
            {
                Debug.Log("[TutorialMachine] Tutorial data is valid. Activating tutorial.");
                _currentStepIndex = 0;
                _isTutorialCompleted = false;
                ActivateStep(_currentStepIndex);
            }
            else
            {
                Debug.Log("[TutorialMachine] Tutorial data is invalid or tutorial is skipped. Disabling tutorial machine.");
                SkipTutorial();
            }
        }

        public void NextStep()
        {
            // Also check the upper bound (_currentStepIndex >= Length) so a re-call after the last
            // step can't hit tutorialSteps[_currentStepIndex] out of range (IndexOutOfRange).
            if (_isTutorialCompleted || TutorialDataInvalid || TutorialStepOutOfBounds)
            {
                return;
            }
            // Ignore duplicate calls while a previous step transition (delay wait) is in flight.
            // Prevents a single tap delivered twice (pass-through + direct click) or a fast double-tap
            // from calling NextStep twice and overrunning the array / skipping a step.
            if (_transitionCoroutine != null)
            {
                return;
            }

            CleanUpCurrentStepListeners();

            bool hasDelay = false;
            float delayDuration = 0f;
            bool transitionAfterDelay = false;
            BackdropType transitionAfterDelayBackdropType = BackdropType.Transparent;

            if (_currentStepIndex >= 0)
            {
                var currentStep = tutorialSteps[_currentStepIndex];
                OnStepComplete(currentStep.onCompleted);
                onTutorialStepComplete?.Invoke();

                if (currentStep.transitionDelay)
                {
                    hasDelay = true;
                    delayDuration = currentStep.transitionDelayDuration;
                    transitionAfterDelay = currentStep.transitionAfterDelay;
                    transitionAfterDelayBackdropType = currentStep.transitionAfterDelayBackdropType;
                    if (TutorialBackdrop.Instance != null)
                    {
                        float alpha = currentStep.transitionDelayBackdropType == BackdropType.Transparent ? 0f : 1f;
                        TutorialBackdrop.Instance.ForceSetup(alpha, true, false);
                    }
                }
            }

            _currentStepIndex++;

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            if (hasDelay && delayDuration > 0f)
            {
                _transitionCoroutine = StartCoroutine(DelayNextStepTransition(
                    _currentStepIndex,
                    delayDuration,
                    transitionAfterDelay,
                    transitionAfterDelayBackdropType
                ));
            }
            else
            {
                ExecuteNextStepTransition(_currentStepIndex);
            }
        }

        private IEnumerator DelayNextStepTransition(int nextIndex, float duration, bool transitionAfterDelay, BackdropType transitionAfterDelayBackdropType)
        {
            yield return new WaitForSeconds(duration);
            _transitionCoroutine = null;

            if (transitionAfterDelay && TutorialBackdrop.Instance != null)
            {
                float alpha = transitionAfterDelayBackdropType == BackdropType.Transparent ? 0f : 1f;
                TutorialBackdrop.Instance.ForceSetup(alpha, true, false);
            }

            ExecuteNextStepTransition(nextIndex);
        }

        private void ExecuteNextStepTransition(int nextIndex)
        {
            if (TutorialStepOutOfBounds)
            {
                Debug.LogWarning($"[TutorialMachine] NextStep called but current step index {_currentStepIndex} is out of bounds. Tutorial will be completed.");
                CompleteTutorial();
                return;
            }
            ActivateStep(nextIndex);
        }
        public void OnStepComplete(OnTutorialStepComplete action)
        {
            // Safely ignore when the index is out of range (guards boundary cases / external calls).
            if (tutorialSteps == null || _currentStepIndex < 0 || _currentStepIndex >= tutorialSteps.Length) return;

            var currentStep = tutorialSteps[_currentStepIndex];
            DisableStepOverlay(currentStep);

            switch (action)
            {
                case OnTutorialStepComplete.DisableTutorialOnTarget:
                    if (currentStep.target != null)
                    {
                        currentStep.target.DisableTutorial();
                    }
                    ClearActivePopup();
                    break;
                case OnTutorialStepComplete.Feedback:
                    if (!_isIgnoreTutorialFeedback && currentStep.target != null)
                    {
                        currentStep.target.StepFeedback();
                    }

                    currentStep.onCompletedFeedback?.Invoke();
                    break;
                default:
                    break;
            }
        }

        private void ActivateStep(int index)
        {
            Debug.Log($"[TutorialMachine] Activating tutorial step {index}.");
            if (tutorialSteps == null || index < 0 || index >= tutorialSteps.Length)
            {
                return;
            }
            var currentStep = tutorialSteps[index];

            if (currentStep.showHighlight && currentStep.target != null)
            {
                currentStep.target.HighlightTarget();
            }

            if (currentStep.showStandout && currentStep.target != null)
            {
                currentStep.target.StandOutTarget();
            }

            if (currentStep.showSpotLight && currentStep.target != null)
            {
                currentStep.target.SpotlightTarget(currentStep.spotLightRadius);
            }

            if (TutorialBackdrop.Instance != null)
            {
                TutorialBackdrop.Instance.SetTutorialText(currentStep.showText, currentStep.instructionText);
            }

            EnableStepOverlay(currentStep);

            ClearActivePopup();
            if (currentStep.showPopup && currentStep.pp_popup != null)
            {
                Vector3 popupPosition = transform.position + currentStep.popupOffset;
                if (currentStep.target != null)
                {
                    popupPosition = currentStep.target.transform.position + currentStep.popupOffset;
                }

                _activePopup = Instantiate(currentStep.pp_popup, popupPosition, Quaternion.identity, transform);
            }

            _nextStepTriggerType = currentStep.nextStepTriggerType;

            if (_nextStepTriggerType == NextStepTriggerType.TouchTutorialTarget && currentStep.target != null)
            {
                var button = currentStep.target.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(OnTargetClicked);
                }
            }
        }

        public void DisableAllSteps()
        {
            if (tutorialSteps == null) return;
            foreach (var step in tutorialSteps)
            {
                DisableStepOverlay(step);

                if (step.target != null)
                {
                    step.target.DisableTutorial();
                }
            }

            ClearActivePopup();
            if (TutorialBackdrop.Instance != null)
            {
                TutorialBackdrop.Instance.SetTutorialText(false, null);
            }
        }

        private static void EnableStepOverlay(TutorialStep step)
        {
            if (!step.showOverlay || step.overlay == null)
            {
                return;
            }

            step.overlay.SetActive(true);
        }

        private static void DisableStepOverlay(TutorialStep step)
        {
            if (step.overlay == null)
            {
                return;
            }

            step.overlay.SetActive(false);
        }

        private void ClearActivePopup()
        {
            if (_activePopup == null)
            {
                return;
            }

            Destroy(_activePopup);
            _activePopup = null;
        }
        public void SkipTutorial()
        {
            if (_isTutorialCompleted)
            {
                return;
            }
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }
            CleanUpCurrentStepListeners();
            DisableAllSteps();
            ClearActivePopup();
            if (TutorialBackdrop.Instance != null)
            {
                TutorialBackdrop.Instance.SetTutorialText(false, null);
                TutorialBackdrop.Instance.HideStandout();
            }
            _isTutorialCompleted = true;
            onTutorialCompleted?.Invoke();
            this.enabled = false;
        }

        private void CompleteTutorial()
        {
            if (_isTutorialCompleted)
            {
                return;
            }
            onTutorialCompleted?.Invoke(); // Invoke the only event that happen when done tutorial
            SkipTutorial();
        }

        private void Update()
        {
            if (_isTutorialCompleted) return;

            if (_nextStepTriggerType == NextStepTriggerType.TouchAnyWhere)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    NextStep();
                }
            }
        }

        private void OnTargetClicked()
        {
            if (_nextStepTriggerType == NextStepTriggerType.TouchTutorialTarget)
            {
                NextStep();
            }
        }

        private void CleanUpCurrentStepListeners()
        {
            if (_currentStepIndex >= 0 && tutorialSteps != null && _currentStepIndex < tutorialSteps.Length)
            {
                var step = tutorialSteps[_currentStepIndex];
                if (step.target != null)
                {
                    var button = step.target.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.RemoveListener(OnTargetClicked);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            CleanUpCurrentStepListeners();
        }
    }

    public enum OnTutorialStepComplete
    {
        None,
        DisableTutorialOnTarget,
        Feedback,
    }
    public enum NextStepTriggerType
    {
        None,
        TouchTutorialTarget,
        TouchAnyWhere,
    }
    public enum BackdropType
    {
        Transparent,
        Dimmed,

    }
}