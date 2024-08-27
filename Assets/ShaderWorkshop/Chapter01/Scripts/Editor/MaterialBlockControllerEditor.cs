using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShaderWorkshop
{
    [CustomEditor(typeof(MaterialBlockController))]
    public class MaterialBlockControllerEditor : Editor
    {
        private bool foldout = false;
        override public void OnInspectorGUI()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Material Block Controller");
            if (foldout)
                base.OnInspectorGUI();
            MaterialBlockController controller = (MaterialBlockController)target;
            if (GUILayout.Button("Collect Renderers"))
            {
                controller.CollectRenderers();
                // set dirty
                EditorUtility.SetDirty(controller);
            }
            // Draw renderers list
            var propRenderers = serializedObject.FindProperty("renderers");
            EditorGUILayout.PropertyField(propRenderers, true);
            
            // Draw custom datas
            // Draw one by one and then Add and Remove buttons
            var propDatas = serializedObject.FindProperty("datas");
            for (int i = 0; i < propDatas.arraySize; i++)
            {
                var propData = propDatas.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Data {i}");
                if (GUILayout.Button("▲", GUILayout.Width(20)))
                {
                    propDatas.MoveArrayElement(i, i - 1);
                    break;
                }
                if (GUILayout.Button("▼", GUILayout.Width(20)))
                {
                    propDatas.MoveArrayElement(i, i + 1);
                    break;
                }
                // Insert
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    propDatas.InsertArrayElementAtIndex(propDatas.arraySize);
                    // move to i + 1
                    propDatas.MoveArrayElement(propDatas.arraySize - 1, i + 1);
                    break;
                }
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    propDatas.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                var propUpdateMethod = propData.FindPropertyRelative("updateMethod");
                EditorGUILayout.PropertyField(propUpdateMethod);
                var propPropertyName = propData.FindPropertyRelative("propertyName");
                EditorGUILayout.PropertyField(propPropertyName);
                var propPropertyType = propData.FindPropertyRelative("propertyType");
                EditorGUILayout.PropertyField(propPropertyType);
                // Check property type and draw the corresponding property
                switch (propPropertyType.enumValueIndex)
                {
                    case (int)MaterialBlockController.PropertyType.Vector:
                        var propVectorValue = propData.FindPropertyRelative("vectorValue");
                        EditorGUILayout.PropertyField(propVectorValue);
                        break;
                    case (int)MaterialBlockController.PropertyType.Float:
                        var propFloatValue = propData.FindPropertyRelative("floatValue");
                        EditorGUILayout.PropertyField(propFloatValue);
                        break;
                    case (int)MaterialBlockController.PropertyType.Color:
                        var propColorValue = propData.FindPropertyRelative("colorValue");
                        EditorGUILayout.PropertyField(propColorValue);
                        break;
                    case (int)MaterialBlockController.PropertyType.World_Position:
                        var propTargetTransform = propData.FindPropertyRelative("targetTransform");
                        EditorGUILayout.PropertyField(propTargetTransform);
                        break;
                }
                EditorGUI.indentLevel--;
            }
            if (GUILayout.Button("Add Data list"))
            {
                propDatas.InsertArrayElementAtIndex(propDatas.arraySize);
            }
            if (GUILayout.Button("Remove Last Data"))
            {
                propDatas.DeleteArrayElementAtIndex(propDatas.arraySize - 1);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}