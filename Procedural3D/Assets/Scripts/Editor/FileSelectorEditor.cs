/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Diogo de Andrade
 * */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FileSelector))]
public class FileSelectorEditor : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var propFilename = property.FindPropertyRelative("filename");

        EditorGUI.BeginProperty(position, label, property);

        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Rect fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - 20, position.height);
        Rect buttonRect = new Rect(fieldRect.xMax, position.y, 20, position.height);

        EditorGUI.LabelField(labelRect, label);

        EditorGUI.BeginDisabledGroup(true);  // Disables editing, not interaction
        EditorGUI.TextField(fieldRect, propFilename.stringValue);
        EditorGUI.EndDisabledGroup();

        bool changeFile = false;

        if (GUI.Button(buttonRect, new GUIContent("...", "Select file")))
        {
            changeFile = true;
        }
        else if ((Event.current.type == EventType.MouseDown) && (Event.current.button == 0) && (fieldRect.Contains(Event.current.mousePosition)))
        {
            changeFile = true;
        }

        EditorGUI.EndProperty();

        if (changeFile)
        {
            string path = EditorUtility.OpenFilePanel("Select a file", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Change File Path");
                propFilename.stringValue = path;
                propFilename.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(property.serializedObject.targetObject);

                // Find the OnPathChangedAttribute on the field that this drawer represents
                OnPathChangedAttribute attribute = FindAttribute<OnPathChangedAttribute>(property);

                if (attribute != null)
                {
                    // Invoke the callback method
                    InvokeCallback(property.serializedObject.targetObject, attribute.Callback, path);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);

        var propFilename = property.FindPropertyRelative("filename");

        return base.GetPropertyHeight(propFilename, label);
    }

    private T FindAttribute<T>(SerializedProperty property) where T : Attribute
    {
        // Get the field info and then the attributes of this field
        var targetObject = property.serializedObject.targetObject;
        var targetType = targetObject.GetType();
        var field = targetType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (field != null)
        {
            return field.GetCustomAttribute<T>();
        }
        return null;
    }

    private void InvokeCallback(UnityEngine.Object targetObject, string methodName, string parameter)
    {
        var method = targetObject.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (method != null)
        {
            method.Invoke(targetObject, new object[] { parameter });
        }
        else
        {
            Debug.LogError("Method '" + methodName + "' not found in " + targetObject.GetType().Name);
        }
    }
}
