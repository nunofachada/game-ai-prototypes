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

        private (int min, int max) bufferSize;

        private void Awake()
        {
            view = viewGameObject.GetComponent<IView>();

            buffer = new LinkedList<TimedInput>();

            bufferSize = (int.MaxValue, 0);
        }

        private void Start()
        {
            bufferSize = (patterns.MinLength, patterns.MaxLength);

            view.SetKnownInputs(patterns.KnownInputs);
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
                AttackType? attack = patterns.Match(buffer);
                if (attack.HasValue)
                {
                    // Action found, schedule it
                    buffer.Clear();
                    Debug.Log(attack);
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