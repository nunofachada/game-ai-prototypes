
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using System.Collections.Generic;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Helper class for rendering a dropdown menu in Unity's IMGUI mode.
    /// </summary>
    public class Dropdown
    {
        private GUIStyle italicTextStyle = null;
        private Vector2 scrollViewVector = Vector2.zero;

        // Render dropdown menu
        public void Show(
            Rect rect, IList<object> options, ref bool show, ref int index,
            string selString = "Select")
        {
            // Instantiate a GUI style for italic fonts
            // Unfortunately this can't be done outside the OnGUI() method, from
            // which this method as to be called from
            if (italicTextStyle == null)
            {
                italicTextStyle = new GUIStyle(GUI.skin.button);
                italicTextStyle.fontStyle = FontStyle.Italic;
            }

            // Is the dropdown menu to be shown?
            if (show)
            {
                // Render button with the selection string in italic
                if (GUI.Button(
                    new Rect(rect.x, rect.y, rect.width, rect.height),
                    selString, italicTextStyle))
                {
                    // If button is pressed, the dropdown menu will be closed
                    show = false;
                }

                // Start the scroll view for the dropdown menu
                scrollViewVector = GUI.BeginScrollView(
                    new Rect(
                        rect.x, rect.y + rect.height,
                        rect.width,
                        Mathf.Min(
                            rect.height * options.Count,
                            Screen.height - rect.y - 2 * rect.height)),
                    scrollViewVector,
                    new Rect(
                        0,
                        0,
                        rect.width,
                        rect.height * options.Count));

                // Render a box which will contain buttons for each option
                GUI.Box(
                    new Rect(0, 0, rect.width, rect.height * options.Count), "");

                // Render a button for each option
                for (int i = 0; i < options.Count; i++)
                {
                    if (GUI.Button(
                        new Rect(0, i * rect.height, rect.width, rect.height),
                        options[i].ToString()))
                    {
                        show = false;
                        index = i;
                    }
                }

                // Close scroll view
                GUI.EndScrollView();
            }
            else
            {
                // Dropdown menu is not shown, render only button with the
                // current selection
                if (GUI.Button(
                    new Rect(rect.x, rect.y, rect.width, rect.height),
                    options[index].ToString()))
                {
                    // If button is pressed, then dropdown menu will be shown
                    show = true;
                }
            }
        }
    }
}