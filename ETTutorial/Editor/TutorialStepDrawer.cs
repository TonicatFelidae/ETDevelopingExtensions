using ETEngine;
using UnityEditor;
using UnityEngine;
namespace ETEngine.TutorialSystem
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TutorialStep))]
    public class TutorialStepDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty targetProp = property.FindPropertyRelative("target");
            SerializedProperty animateTargetProp = property.FindPropertyRelative("animateTarget");
            SerializedProperty targetUnclickableDelayProp = property.FindPropertyRelative("targetUnclickableDelay");
            SerializedProperty targetUnclickableDurationProp = property.FindPropertyRelative("targetUnclickableDuration");
            SerializedProperty useBackdropProp = property.FindPropertyRelative("useBackdrop");
            SerializedProperty backdropAlphaProp = property.FindPropertyRelative("backdropAlpha");
            SerializedProperty highlightTargetProp = property.FindPropertyRelative("highlightTarget");
            SerializedProperty spotLightTargetProp = property.FindPropertyRelative("spotLightTarget");
            SerializedProperty spotLightRadiusProp = property.FindPropertyRelative("spotLightRadius");
            SerializedProperty showTextProp = property.FindPropertyRelative("showText");
            SerializedProperty instructionTextProp = property.FindPropertyRelative("instructionText");
            SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
            SerializedProperty popupPrefabProp = property.FindPropertyRelative("pp_popup");
            SerializedProperty showOverlayProp = property.FindPropertyRelative("showOverlay");
            SerializedProperty overlayProp = property.FindPropertyRelative("overlay");
            SerializedProperty popupOffsetProp = property.FindPropertyRelative("popupOffset");
            SerializedProperty nextStepTriggerTypeProp = property.FindPropertyRelative("nextStepTriggerType");
            SerializedProperty nextStepTriggerDelayProp = property.FindPropertyRelative("nextStepTriggerDelay");
            SerializedProperty onCompletedProp = property.FindPropertyRelative("onCompleted");
            SerializedProperty onCompletedFeedbackProp = property.FindPropertyRelative("onCompletedFeedback");
            SerializedProperty transitionDelayProp = property.FindPropertyRelative("transitionDelay");
            SerializedProperty transitionDelayBackdropTypeProp = property.FindPropertyRelative("transitionDelayBackdropType");
            SerializedProperty transitionDelayDurationProp = property.FindPropertyRelative("transitionDelayDuration");
            SerializedProperty transitionAfterDelayProp = property.FindPropertyRelative("transitionAfterDelay");
            SerializedProperty transitionAfterDelayBackdropTypeProp = property.FindPropertyRelative("transitionAfterDelayBackdropType");

            DrawField(ref rect, targetProp, spacing);

            DrawField(ref rect, animateTargetProp, spacing);
            DrawField(ref rect, targetUnclickableDelayProp, spacing);

            if (targetUnclickableDelayProp != null && targetUnclickableDelayProp.boolValue)
            {
                DrawField(ref rect, targetUnclickableDurationProp, spacing);
            }

            DrawField(ref rect, useBackdropProp, spacing);

            if (useBackdropProp != null && useBackdropProp.boolValue)
            {
                DrawField(ref rect, backdropAlphaProp, spacing);
                DrawField(ref rect, highlightTargetProp, spacing);
            }

            DrawField(ref rect, spotLightTargetProp, spacing);

            if (spotLightTargetProp != null && spotLightTargetProp.boolValue)
            {
                DrawField(ref rect, spotLightRadiusProp, spacing);
            }

            DrawField(ref rect, showTextProp, spacing);

            if (showTextProp != null && showTextProp.boolValue)
            {
                DrawField(ref rect, instructionTextProp, spacing);
            }

            DrawField(ref rect, showPopupProp, spacing);

            if (showPopupProp != null && showPopupProp.boolValue)
            {
                DrawField(ref rect, popupPrefabProp, spacing);
                DrawField(ref rect, popupOffsetProp, spacing);
            }

            DrawField(ref rect, showOverlayProp, spacing);

            if (showOverlayProp != null && showOverlayProp.boolValue)
            {
                DrawField(ref rect, overlayProp, spacing);
            }

            DrawField(ref rect, nextStepTriggerTypeProp, spacing);
            if (nextStepTriggerTypeProp != null && (NextStepTriggerType)nextStepTriggerTypeProp.enumValueIndex == NextStepTriggerType.Delay)
            {
                DrawField(ref rect, nextStepTriggerDelayProp, spacing);
            }

            DrawField(ref rect, onCompletedProp, spacing);

            if (onCompletedProp != null && (OnTutorialStepComplete)onCompletedProp.enumValueIndex == OnTutorialStepComplete.Feedback)
            {
                DrawField(ref rect, onCompletedFeedbackProp, spacing);
            }

            DrawField(ref rect, transitionDelayProp, spacing);
            if (transitionDelayProp != null && transitionDelayProp.boolValue)
            {
                DrawField(ref rect, transitionDelayBackdropTypeProp, spacing);
                DrawField(ref rect, transitionDelayDurationProp, spacing);
            }

            DrawField(ref rect, transitionAfterDelayProp, spacing);
            if (transitionAfterDelayProp != null && transitionAfterDelayProp.boolValue)
            {
                DrawField(ref rect, transitionAfterDelayBackdropTypeProp, spacing);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = 0f;

            height += GetFieldHeight(property.FindPropertyRelative("target"), spacing);
            height += GetFieldHeight(property.FindPropertyRelative("animateTarget"), spacing);
            height += GetFieldHeight(property.FindPropertyRelative("targetUnclickableDelay"), spacing);

            SerializedProperty targetUnclickableDelayProp = property.FindPropertyRelative("targetUnclickableDelay");
            if (targetUnclickableDelayProp != null && targetUnclickableDelayProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("targetUnclickableDuration"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("useBackdrop"), spacing);

            SerializedProperty useBackdropProp = property.FindPropertyRelative("useBackdrop");
            if (useBackdropProp != null && useBackdropProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("backdropAlpha"), spacing);
                height += GetFieldHeight(property.FindPropertyRelative("highlightTarget"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("spotLightTarget"), spacing);

            SerializedProperty spotLightTargetProp = property.FindPropertyRelative("spotLightTarget");
            if (spotLightTargetProp != null && spotLightTargetProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("spotLightRadius"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("showText"), spacing);

            SerializedProperty showTextProp = property.FindPropertyRelative("showText");
            if (showTextProp != null && showTextProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("instructionText"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("showPopup"), spacing);

            SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
            if (showPopupProp != null && showPopupProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("pp_popup"), spacing);
                height += GetFieldHeight(property.FindPropertyRelative("popupOffset"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("showOverlay"), spacing);

            SerializedProperty showOverlayProp = property.FindPropertyRelative("showOverlay");
            if (showOverlayProp != null && showOverlayProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("overlay"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("nextStepTriggerType"), spacing);

            SerializedProperty nextStepTriggerTypeProp = property.FindPropertyRelative("nextStepTriggerType");
            if (nextStepTriggerTypeProp != null && (NextStepTriggerType)nextStepTriggerTypeProp.enumValueIndex == NextStepTriggerType.Delay)
            {
                height += GetFieldHeight(property.FindPropertyRelative("nextStepTriggerDelay"), spacing);
            }

            SerializedProperty onCompletedProp = property.FindPropertyRelative("onCompleted");
            height += GetFieldHeight(onCompletedProp, spacing);

            if (onCompletedProp != null && (OnTutorialStepComplete)onCompletedProp.enumValueIndex == OnTutorialStepComplete.Feedback)
            {
                height += GetFieldHeight(property.FindPropertyRelative("onCompletedFeedback"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("transitionDelay"), spacing);
            SerializedProperty transitionDelayProp = property.FindPropertyRelative("transitionDelay");
            if (transitionDelayProp != null && transitionDelayProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("transitionDelayBackdropType"), spacing);
                height += GetFieldHeight(property.FindPropertyRelative("transitionDelayDuration"), spacing);
            }

            height += GetFieldHeight(property.FindPropertyRelative("transitionAfterDelay"), spacing);
            SerializedProperty transitionAfterDelayProp = property.FindPropertyRelative("transitionAfterDelay");
            if (transitionAfterDelayProp != null && transitionAfterDelayProp.boolValue)
            {
                height += GetFieldHeight(property.FindPropertyRelative("transitionAfterDelayBackdropType"), spacing);
            }

            return Mathf.Max(0f, height - spacing);
        }

        private static void DrawField(ref Rect rect, SerializedProperty prop, float spacing)
        {
            if (prop == null)
            {
                return;
            }

            EditorGUI.PropertyField(rect, prop, true);
            rect.y += EditorGUI.GetPropertyHeight(prop, true) + spacing;
        }

        private static float GetFieldHeight(SerializedProperty prop, float spacing)
        {
            if (prop == null)
            {
                return 0f;
            }

            return EditorGUI.GetPropertyHeight(prop, true) + spacing;
        }
    }
#endif
}