using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private GameObject viewGameObject;

        [SerializeField]
        private float keyValidDuration = 1f;

        [SerializeField]
        [ReorderableList]
        private List<AttackPattern> patterns;

        private LinkedList<TimedInput> buffer;
        private IView view;
        private PatternTreeNode patternMatchTreeRoot;

        private (int min, int max) bufferSize;

        private void Awake()
        {
            view = viewGameObject.GetComponent<IView>();

            buffer = new LinkedList<TimedInput>();

            patternMatchTreeRoot = new PatternTreeNode();

            bufferSize = (int.MaxValue, 0);
        }

        private void Start()
        {
            ISet<string> validInputs = new HashSet<string>();

            foreach (AttackPattern pattern in patterns)
            {
                IReadOnlyCollection<string> patStrings = pattern.Preprocess();
                if (patStrings.Count < bufferSize.min) bufferSize.min = patStrings.Count;
                if (patStrings.Count > bufferSize.max) bufferSize.max = patStrings.Count;
                validInputs.UnionWith(patStrings);
                patternMatchTreeRoot.AddPattern(pattern);
            }

            view.SetValidInputs(validInputs);
        }

        private void OnEnable()
        {
            view.OnPressedInput += HandleInput;
        }

        private void OnDisable()
        {
            view.OnPressedInput -= HandleInput;
        }

        private void Update()
        {
            if (buffer.Count > 0)
            {
                while (Time.time > buffer.First.Value.Time + keyValidDuration)
                {
                    buffer.RemoveFirst();
                }
            }
        }

        private void FixedUpdate()
        {
            if (buffer.Count >= bufferSize.min && buffer.Count <= bufferSize.max)
            {
                PatternTreeNode attackNode = patternMatchTreeRoot.Match(buffer);
                if (attackNode is null)
                {
                    // Input didn't match anything
                }
                else if (attackNode.IsLeaf)
                {
                    // Action found, schedule it
                    Debug.Log(attackNode.Attack);
                }
            }
        }

        private void HandleInput(string input)
        {
            buffer.AddLast(new TimedInput(Time.time, input));
            if (buffer.Count > bufferSize.max)
            {
                buffer.RemoveFirst();
            }
            Debug.Log($"Pressed '{input}'");
        }

        [Button]
        private void ClearPatterns()
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

        [Button]
        private void SetPatternsToDefault()
        {
            ClearPatterns();

            // Low attack
            patterns.Add(new AttackPattern("d,s,s,s", AttackType.Low));
            // Med attack
            patterns.Add(new AttackPattern("d,a,d,d", AttackType.Med));
            // High attack
            patterns.Add(new AttackPattern("d,w,w,w", AttackType.High));
            // Mega attack
            patterns.Add(new AttackPattern("d,w,w,d,d", AttackType.Mega));
            // Super attack
            patterns.Add(new AttackPattern("d,a,d,w,w", AttackType.Super));
            // Hyper attack
            patterns.Add(new AttackPattern("d,s,s,w,s", AttackType.Hyper));

        }
    }
}