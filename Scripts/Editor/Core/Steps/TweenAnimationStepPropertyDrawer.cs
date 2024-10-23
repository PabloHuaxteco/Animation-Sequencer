#if DOTWEEN_ENABLED
using System;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    // Modified by Pablo Huaxteco
    [CustomPropertyDrawer(typeof(TweenAnimationStep))]
    public class TweenAnimationStepPropertyDrawer : AnimationStepBasePropertyDrawer
    {
        private void AddNewActionOfType(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            actionsSerializedProperty.arraySize++;
            SerializedProperty arrayElement = actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 1);
            arrayElement.managedReferenceValue = Activator.CreateInstance(targetType);

            if (actionsSerializedProperty.arraySize > 1)
            {
                SerializedProperty previousElement = actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 2);

                if (AnimationControllerDefaults.Instance.PreferUsingPreviousDirection)
                {
                    SerializedProperty previousDirection = previousElement.FindPropertyRelative("direction");
                    if (previousDirection != null)
                    {
                        SerializedProperty currentDirection = arrayElement.FindPropertyRelative("direction");
                        if (currentDirection != null)
                            currentDirection.enumValueIndex = previousDirection.enumValueIndex;
                    }
                }

                if (AnimationControllerDefaults.Instance.PreferUsingPreviousActionEasing)
                {
                    SerializedProperty previousEase = previousElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                    if (previousEase != null)
                    {
                        SerializedProperty currentEase = arrayElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                        if (currentEase != null)
                            currentEase.enumValueIndex = previousEase.enumValueIndex;
                    }
                }
                else
                {
                    SerializedProperty currentEase = arrayElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                    if (currentEase != null)
                        currentEase.enumValueIndex = (int)AnimationControllerDefaults.Instance.DefaultEasing.Ease;
                }

                if (AnimationControllerDefaults.Instance.PreferUsingPreviousRelativeValue)
                {
                    SerializedProperty previousEase = previousElement.FindPropertyRelative("isRelative");
                    if (previousEase != null)
                    {
                        SerializedProperty currentEase = arrayElement.FindPropertyRelative("isRelative");
                        if (currentEase != null)
                            currentEase.boolValue = previousEase.boolValue;
                    }
                }
                else
                {
                    SerializedProperty currentEase = arrayElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                    if (currentEase != null)
                        currentEase.enumValueIndex = (int)AnimationControllerDefaults.Instance.DefaultEasing.Ease;
                }
            }
            else
            {
                SerializedProperty currentEase = arrayElement.FindPropertyRelative("ease").FindPropertyRelative("ease");
                if (currentEase != null)
                    currentEase.enumValueIndex = (int)AnimationControllerDefaults.Instance.DefaultEasing.Ease;

                SerializedProperty currentDirection = arrayElement.FindPropertyRelative("direction");
                if (currentDirection != null)
                    currentDirection.enumValueIndex = (int)AnimationControllerDefaults.Instance.DefaultDirection;

                SerializedProperty isRelativeSerializedProperty = arrayElement.FindPropertyRelative("isRelative");
                if (isRelativeSerializedProperty != null)
                    isRelativeSerializedProperty.boolValue = AnimationControllerDefaults.Instance.UseRelative;
            }

            actionsSerializedProperty.isExpanded = true;
            actionsSerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label, "actions", "loopCount", "loopType");

            float originHeight = position.y;
            if (property.isExpanded)
            {
                if (EditorGUI.indentLevel > 0)
                    position = EditorGUI.IndentedRect(position);

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;

                SerializedProperty flowTypeSerializedProperty = property.FindPropertyRelative("flowType");
                FlowType flowType = (FlowType)flowTypeSerializedProperty.enumValueIndex;
                if (flowType == FlowType.Join)
                {
                    EditorGUI.indentLevel++;
                    position = EditorGUI.IndentedRect(position);
                    EditorGUI.indentLevel--;
                }

                position.y += base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginChangeCheck();
                SerializedProperty actionsSerializedProperty = property.FindPropertyRelative("actions");
                SerializedProperty targetSerializedProperty = property.FindPropertyRelative("target");
                SerializedProperty loopCountSerializedProperty = property.FindPropertyRelative("loopCount");
                EditorGUI.PropertyField(position, loopCountSerializedProperty);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                loopCountSerializedProperty.intValue = Mathf.Clamp(loopCountSerializedProperty.intValue, -1, int.MaxValue);
                if (loopCountSerializedProperty.intValue != 0)
                {
                    if (loopCountSerializedProperty.intValue == -1)
                    {
                        Debug.LogWarning("Infinity Loops doesn't work well with sequence, the best way of doing " +
                                         "that is setting to the int.MaxValue, will end eventually, but will take a really " +
                                         "long time, more info here: https://github.com/Demigiant/dotween/issues/92");
                        loopCountSerializedProperty.intValue = int.MaxValue;
                    }
                    SerializedProperty loopTypeSerializedProperty = property.FindPropertyRelative("loopType");
                    EditorGUI.PropertyField(position, loopTypeSerializedProperty);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                
                position.y += EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight * 1.15f;
                float originalWidth = position.width;
                Rect actionsFoldoutPosition = position;
                actionsFoldoutPosition.x += 10;
                actionsFoldoutPosition.width = EditorGUIUtility.labelWidth - 10;
                actionsSerializedProperty.isExpanded = EditorGUI.Foldout(actionsFoldoutPosition, actionsSerializedProperty.isExpanded, "Actions", true, EditorStyles.foldout);

                position.x += EditorGUIUtility.labelWidth;
                position.width = originalWidth - EditorGUIUtility.labelWidth;
                if (GUI.Button(position, "+"))
                {
                    AnimationSequenceEditorGUIUtility.TweenActionsDropdown.Show(position, actionsSerializedProperty, targetSerializedProperty.objectReferenceValue,
                        item =>
                        {
                            if (AnimationSequenceEditorGUIUtility.TweenActionsDropdown.IsTypeAlreadyInUse(actionsSerializedProperty, item.BaseTweenActionType))
                                Debug.Log($"The '{item.name}' action already exists in this step.");
                            else
                                AddNewActionOfType(actionsSerializedProperty, item.BaseTweenActionType);
                        });
                }
                position.x -= EditorGUIUtility.labelWidth;
                position.width = originalWidth;
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                if (actionsSerializedProperty.isExpanded)
                {
                    int arraySize = actionsSerializedProperty.arraySize;
                    for (int i = 0; i < arraySize; i++)
                    {
                        if (DrawDeleteActionButton(position, property, i))
                        {
                            SerializedProperty actionSerializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(i);

                            bool guiEnabled = GUI.enabled;

                            if (GUI.enabled)
                            {
                                bool isValidTargetForRequiredComponent = IsValidTargetForRequiredComponent(targetSerializedProperty, actionSerializedProperty);
                                GUI.enabled = isValidTargetForRequiredComponent;
                            }

                            EditorGUI.PropertyField(position, actionSerializedProperty);
                            position.y += actionSerializedProperty.GetPropertyDrawerHeight();

                            if (i < arraySize - 1)
                                position.y += EditorGUIUtility.standardVerticalSpacing;

                            GUI.enabled = guiEnabled;
                        }
                        else
                        {
                            i--;
                            arraySize--;
                        }
                    }
                }

                EditorGUI.indentLevel--;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel++;

                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
            property.SetPropertyDrawerHeight(position.y - originHeight + (property.isExpanded ? 0 : EditorGUIUtility.singleLineHeight));
        }

        private static bool IsValidTargetForRequiredComponent(SerializedProperty targetSerializedProperty, SerializedProperty actionSerializedProperty)
        {
            if (targetSerializedProperty.objectReferenceValue == null)
                return false;

            Type type = actionSerializedProperty.GetTypeFromManagedFullTypeName();
            return AnimationSequenceEditorGUIUtility.CanActionBeAppliedToTarget(type, targetSerializedProperty.objectReferenceValue as GameObject);
        }

        private bool DrawDeleteActionButton(Rect position, SerializedProperty property, int targetIndex)
        {
            Rect buttonPosition = position;
            buttonPosition.width = 24;
            buttonPosition.x += position.width - 34;
            buttonPosition.y += 10;

            if (GUI.Button(buttonPosition, "X", EditorStyles.miniButton))
            {
                DeleteElementAtIndex(property, targetIndex);
                return false;
            }

            return true;
        }

        private void DeleteElementAtIndex(SerializedProperty serializedProperty, int targetIndex)
        {
            SerializedProperty actionsPropertyPath = serializedProperty.FindPropertyRelative("actions");
            actionsPropertyPath.DeleteArrayElementAtIndex(targetIndex);
            SerializedPropertyExtensions.ClearPropertyCache(actionsPropertyPath.propertyPath);
            //actionsPropertyPath.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }
    }
}
#endif