using UnityEngine;

#if UNITY_EDITOR
namespace ET
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ET2DLightEffectData))]
    public class ET2DLightEffectDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineH = EditorGUIUtility.singleLineHeight;
            float pad = EditorGUIUtility.standardVerticalSpacing;
            float height = 0f;

            // Foldout header
            height += lineH + pad;

            if (property.isExpanded)
            {
                // enableFlicker
                height += lineH + pad;
                var enableFlickerProp = property.FindPropertyRelative("enableFlicker");
                if (enableFlickerProp != null && enableFlickerProp.boolValue)
                {
                    height += (lineH + pad) * 3;
                }

                // enableColorShift
                height += lineH + pad;
                var enableColorShiftProp = property.FindPropertyRelative("enableColorShift");
                if (enableColorShiftProp != null && enableColorShiftProp.boolValue)
                {
                    height += (lineH + pad) * 3; // Mode, Speed, Gradient
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var enableFlickerProp = property.FindPropertyRelative("enableFlicker");
            var enableColorShiftProp = property.FindPropertyRelative("enableColorShift");

            string flickerStatus = (enableFlickerProp != null && enableFlickerProp.boolValue) ? "On" : "Off";
            string colorShiftStatus = (enableColorShiftProp != null && enableColorShiftProp.boolValue) ? "On" : "Off";
            string headerText = $"{label.text} (Flicker: {flickerStatus}, Color Shift: {colorShiftStatus})";

            float lineH = EditorGUIUtility.singleLineHeight;
            float pad = EditorGUIUtility.standardVerticalSpacing;
            Rect foldoutRect = new Rect(position.x, position.y, position.width, lineH);

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, headerText, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float currentY = position.y + lineH + pad;

                // 1. enableFlicker
                Rect rFlicker = new Rect(position.x, currentY, position.width, lineH);
                EditorGUI.PropertyField(rFlicker, enableFlickerProp);
                currentY += lineH + pad;

                if (enableFlickerProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    
                    SerializedProperty flickerSpeedProp = property.FindPropertyRelative("flickerSpeed");
                    Rect rSpeed = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rSpeed, flickerSpeedProp);
                    currentY += lineH + pad;

                    SerializedProperty flickerIntensityProp = property.FindPropertyRelative("flickerIntensity");
                    Rect rIntensity = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rIntensity, flickerIntensityProp);
                    currentY += lineH + pad;

                    SerializedProperty flickerRandomnessProp = property.FindPropertyRelative("flickerRandomness");
                    Rect rRandomness = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rRandomness, flickerRandomnessProp);
                    currentY += lineH + pad;
                    
                    EditorGUI.indentLevel--;
                }

                // 2. enableColorShift
                Rect rColorShift = new Rect(position.x, currentY, position.width, lineH);
                EditorGUI.PropertyField(rColorShift, enableColorShiftProp);
                currentY += lineH + pad;

                if (enableColorShiftProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    
                    SerializedProperty colorShiftModeProp = property.FindPropertyRelative("colorShiftMode");
                    Rect rShiftMode = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rShiftMode, colorShiftModeProp);
                    currentY += lineH + pad;

                    SerializedProperty colorShiftSpeedProp = property.FindPropertyRelative("colorShiftSpeed");
                    Rect rShiftSpeed = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rShiftSpeed, colorShiftSpeedProp);
                    currentY += lineH + pad;

                    SerializedProperty colorGradientProp = property.FindPropertyRelative("colorGradient");
                    Rect rGradient = new Rect(position.x, currentY, position.width, lineH);
                    EditorGUI.PropertyField(rGradient, colorGradientProp);
                    currentY += lineH + pad;
                    
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
