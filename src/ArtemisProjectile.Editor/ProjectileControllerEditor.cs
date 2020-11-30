using UnityEngine;
using UnityEditor;

namespace ArtemisProjectile
{
    [CustomEditor(typeof(ProjectileController), true)]
    public class ProjectileControllerEditor : Editor
    {
        SerializedProperty speed;
        SerializedProperty gravityMultiplier;
        SerializedProperty layerMask;
        SerializedProperty updateLoop;

        SerializedProperty penetrationEnabled;
        SerializedProperty penetration;

        SerializedProperty ricochetEnabled;
        SerializedProperty ricochetAngle;

        SerializedProperty debugEnabled;
        SerializedProperty debugPathSurvivesDestroy;
        SerializedProperty pathColor;
        SerializedProperty normalColor;
        SerializedProperty penetrationColor;

        bool penetrationFoldout = true;
        bool ricochetFoldout = true;
        bool debugFoldout = true;

        public void OnEnable()
        {
            speed = serializedObject.FindProperty("_speed");
            gravityMultiplier = serializedObject.FindProperty("_gravityMultiplier");
            layerMask = serializedObject.FindProperty("_layerMask");
            updateLoop = serializedObject.FindProperty("_updateLoop");

            penetrationEnabled = serializedObject.FindProperty("_penetrationEnabled");
            penetration = serializedObject.FindProperty("_penetration");

            ricochetEnabled = serializedObject.FindProperty("_ricochetEnabled");
            ricochetAngle = serializedObject.FindProperty("_ricochetAngle");

            debugEnabled = serializedObject.FindProperty("_debugEnabled");
            debugPathSurvivesDestroy = serializedObject.FindProperty("_ignoreDestroy");
            pathColor = serializedObject.FindProperty("_pathColor");
            normalColor = serializedObject.FindProperty("_normalColor");
            penetrationColor = serializedObject.FindProperty("_penetrationColor");

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(gravityMultiplier);
            EditorGUILayout.PropertyField(layerMask);
            EditorGUILayout.PropertyField(updateLoop);
            EditorGUILayout.Space();

            //penetration
            penetrationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(penetrationFoldout, "Penetration");
            if (penetrationFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(penetrationEnabled);
                if (penetrationEnabled.boolValue)
                    EditorGUILayout.PropertyField(penetration);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //ricochet
            ricochetFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(ricochetFoldout, "Ricochet");
            if (ricochetFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(ricochetEnabled);

                if (ricochetEnabled.boolValue)
                {
                    var content = new GUIContent()
                    {
                        text = ricochetAngle.displayName,
                        tooltip = ricochetAngle.tooltip
                    };
                    ricochetAngle.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(content, ricochetAngle.floatValue), 0, 90);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //debug
            debugFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(debugFoldout, "Debug");
            if (debugFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(debugEnabled);
                if (debugEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(debugPathSurvivesDestroy);
                    //colors
                    EditorGUILayout.PropertyField(pathColor);
                    EditorGUILayout.PropertyField(normalColor);
                    EditorGUILayout.PropertyField(penetrationColor);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}