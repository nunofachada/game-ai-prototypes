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
        [Expandable]
        private AttackPatternSet patterns;

        [SerializeField]
        private float keyValidDuration = 1.5f;

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
            ISet<KeyCode> validInputs = new HashSet<KeyCode>();

            foreach (AttackPattern pattern in patterns.Patterns)
            {
                Debug.Log(pattern + ", size=" + pattern.Size);
                if (pattern.Size < bufferSize.min) bufferSize.min = pattern.Size;
                if (pattern.Size > bufferSize.max) bufferSize.max = pattern.Size;
                validInputs.UnionWith(pattern.Pattern);
                patternMatchTreeRoot.AddPattern(pattern.ReversePatternEnumerator, pattern.Attack);
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
            while (buffer.Count > 0 && Time.time > buffer.First.Value.Time + keyValidDuration)
            {
                buffer.RemoveFirst();
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
                    buffer.Clear();
                    Debug.Log(attackNode.Attack);
                }
            }
        }

        private void HandleInput(KeyCode input)
        {
            buffer.AddLast(new TimedInput(Time.time, input));
            if (buffer.Count > bufferSize.max)
            {
                buffer.RemoveFirst();
            }
            Debug.Log($"Pressed '{input}' (buffer size is {buffer.Count})");
        }
    }
}