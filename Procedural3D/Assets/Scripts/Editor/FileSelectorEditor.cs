/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Diogo de Andrade
 * */

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
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);

        var propFilename = property.FindPropertyRelative("filename");

        return base.GetPropertyHeight(propFilename, label);
    }
}