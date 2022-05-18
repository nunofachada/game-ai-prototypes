using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private float keyValidDuration = 1.5f;

        private LinkedList<TimedInput> buffer;
        private Patterns patterns;
        private InputFrontend inputFrontend;

        private (int min, int max) bufferSize;

        private void Awake()
        {
            inputFrontend = GetComponentInParent<InputFrontend>();

            patterns = GetComponent<Patterns>();

            buffer = new LinkedList<TimedInput>();
        }

        private void Start()
        {
            bufferSize = (patterns.MinLength, patterns.MaxLength);
        }

        private void OnEnable()
        {
            inputFrontend.OnPressedInput += HandleInput;
        }

        private void OnDisable()
        {
            inputFrontend.OnPressedInput -= HandleInput;
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