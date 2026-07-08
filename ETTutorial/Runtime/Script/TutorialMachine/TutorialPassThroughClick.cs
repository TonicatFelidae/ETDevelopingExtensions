using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace ETEngine.TutorialSystem
{
    [RequireComponent(typeof(Button))]
    public class TutorialPassThroughButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (EventSystem.current == null)
            {
                Debug.LogError($"[TutorialPassThroughButton] No EventSystem found on click for {gameObject.name}.");
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                return;
            }

            bool forwarded = false;

            foreach (var result in results)
            {
                if (result.gameObject == gameObject)
                {
                    continue;
                }

                var tutorialTarget = result.gameObject.GetComponentInParent<TutorialTarget>();
                var targetButton = result.gameObject.GetComponentInParent<Button>();
                if (tutorialTarget == null)
                {
                    continue;
                }

                if (targetButton == null)
                {
                    continue;
                }

                if (!targetButton.interactable)
                {
                    continue;
                }

                if (tutorialTarget.gameObject != targetButton.gameObject)
                {
                    continue;
                }

                targetButton.onClick.Invoke();
                forwarded = true;
                break;
            }

            if (!forwarded)
            {
                Debug.LogWarning($"[TutorialPassThroughButton] Click was not forwarded because no valid TutorialTarget/Button pair was hit.");
            }
        }
    }
}
