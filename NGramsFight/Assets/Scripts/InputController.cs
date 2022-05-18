using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private float keyValidDuration = 1.5f;

        private LinkedList<TimedInput> buffer;
        private MoveConfig moveConfig;
        private InputHandler inputHandler;

        private (int min, int max) bufferSize;

        private void Awake()
        {
            inputHandler = GetComponentInParent<InputHandler>();

            moveConfig = GetComponent<MoveConfig>();

            buffer = new LinkedList<TimedInput>();

            bufferSize = (int.MaxValue, 0);

            bufferSize = (moveConfig.Patterns.MinLength, moveConfig.Patterns.MaxLength);
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            inputHandler.OnPressedInput += HandleInput;
        }

        private void OnDisable()
        {
            inputHandler.OnPressedInput -= HandleInput;
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
                AttackType? attack = moveConfig.Patterns.Match(buffer);
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