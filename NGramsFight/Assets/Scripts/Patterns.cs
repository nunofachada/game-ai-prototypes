// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// Contains all the known patterns and respective attacks, editable in
    /// Unity's editor.
    /// </summary>
    public class Patterns : MonoBehaviour
    {
        // List of attack patterns
        [SerializeField]
        [ReorderableList]
        private List<AttackPattern> patterns;

        // Reference to the input frontend
        private InputFrontend inputFrontend;

        // Reference to the pattern tree root
        private PatternTreeNode patternMatchTreeRoot;

        // Set of known / valid inputs
        private ISet<KeyCode> knownInputs;

        /// <summary>
        /// Length of the smallest pattern.
        /// </summary>
        public int MinLength { get; private set; }

        /// <summary>
        /// Length of the largest pattern.
        /// </summary>
        public int MaxLength { get; private set; }

        /// <summary>
        /// Match an input pattern with a possible attack.
        /// </summary>
        /// <param name="inputList">The input pattern to match.</param>
        /// <returns>
        /// The attack associated with the given input pattern, or `null` if no
        /// match is found.
        /// </returns>
        public AttackType? Match(IReadOnlyList<KeyCode> inputList)
        {
            return patternMatchTreeRoot.Match(inputList)?.Attack;
        }

        // Called when the script instance is being loaded
        private void Awake()
        {
            patternMatchTreeRoot = new PatternTreeNode();
            knownInputs = new HashSet<KeyCode>();
            MinLength = int.MaxValue;
            MaxLength = 0;

            // Cycle through the specified attack patterns and build the pattern
            // tree, used latter for efficient pattern matching
            foreach (AttackPattern pattern in patterns)
            {
                // Keep note of the smallest and largest pattern lengths
                if (pattern.Length < MinLength) MinLength = pattern.Length;
                if (pattern.Length > MaxLength) MaxLength = pattern.Length;

                // Add the current pattern's key codes to the valid key code set
                knownInputs.UnionWith(pattern);

                // Add pattern to the pattern matching tree
                patternMatchTreeRoot.AddPattern(
                    pattern.GetReverseEnumerator(), pattern.Attack);
            }
        }

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            inputFrontend = GetComponentInParent<InputFrontend>();
            inputFrontend.SetKnownInputs(knownInputs);
        }

        // Clear the current attack pattern list
        [Button]
        private void Clear()
        {
            if (patterns is null)
            {
                patterns = new List<AttackPattern>();
            }
            else
            {
                patterns.Clear();
            }
        }

        // Reset the attack pattern list to sane and tested defaults
        [Button]
        private void Reset()
        {
            Clear();

            // Low attack
            patterns.Add(new AttackPattern("d,s,s,s", AttackType.Low));
            // Med attack
            patterns.Add(new AttackPattern("d,a,d,d", AttackType.Med));
            // High attack
            patterns.Add(new AttackPattern("d,w,w,w", AttackType.High));
            // Mega attack
            patterns.Add(new AttackPattern("d,w,s,a,a", AttackType.MegaMean));
            // Super attack
            patterns.Add(new AttackPattern("d,a,a,d,d", AttackType.SuperUnder));
            // Hyper attack
            patterns.Add(new AttackPattern("d,s,a,w,d", AttackType.HyperTop));
        }
    }
}