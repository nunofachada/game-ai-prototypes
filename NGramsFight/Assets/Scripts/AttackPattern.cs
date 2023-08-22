// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// Defines an attack pattern.
    /// </summary>
    /// <remarks>
    /// Instances of this class are to be used within a list in the editor,
    /// therefore the class needs to be serializable, as well as some of the
    /// information it holds.
    /// </remarks>
    [Serializable]
    public class AttackPattern : IEnumerable<KeyCode>
    {
        [Tooltip("Defines a pattern, can be any set of comma-separated Unity KeyCode strings")]
        [SerializeField]
        private string patternStr;

        [Tooltip("Attack to associate with the specified pattern")]
        [SerializeField]
        private AttackType attack;

        // The actual pattern, converted from the string to a list of key codes
        [SerializeField]
        [HideInInspector]
        private List<KeyCode> pattern;

        /// <summary>
        /// The attack associated with this pattern (read-only).
        /// </summary>
        public AttackType Attack => attack;

        /// <summary>
        /// The length of this pattern (read-only).
        /// </summary>
        public int Length => pattern.Count;

        /// <summary>
        /// Creates a new attack pattern.
        /// </summary>
        /// <param name="patternStr">
        /// The pattern string, i.e. a string containing comma-separated Unity
        /// KeyCode strings.
        /// </param>
        /// <param name="attack">
        /// The attack to associate with the given <paramref name="patternStr"/>.
        /// </param>
        public AttackPattern(string patternStr, AttackType attack)
        {
            this.patternStr = patternStr;
            this.attack = attack;

            pattern = new List<KeyCode>();

            // Convert comma-separated string of key codes into an actual list
            // of key codes
            foreach (string patStr in patternStr.Split(','))
            {
                if (!string.IsNullOrEmpty(patStr))
                {
                    pattern.Add(Event.KeyboardEvent(patStr.Trim()).keyCode);
                }
            }
        }

        /// <summary>
        /// Get an enumerator which backwardly iterates through the pattern.
        /// </summary>
        /// <returns>
        /// An enumerator which backwardly iterates through the pattern.
        /// </returns>
        public IEnumerator<KeyCode> GetReverseEnumerator()
        {
            for (int i = pattern.Count - 1; i >= 0; i--)
            {
                yield return pattern[i];
            }
        }

        /// <summary>
        /// Get an enumerator which iterates through the pattern.
        /// </summary>
        /// <returns>
        /// An enumerator which iterates through the pattern.
        /// </returns>
        public IEnumerator<KeyCode> GetEnumerator() => pattern.GetEnumerator();

        // Explicit interface implementation of non-generic IEnumerable.
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets a string representation of this pattern.
        /// </summary>
        /// <returns>
        /// A string representation of this pattern.
        /// </returns>
        public override string ToString()
        {
            return patternStr;
        }
    }
}