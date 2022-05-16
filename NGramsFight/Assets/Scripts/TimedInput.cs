using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public struct TimedInput
    {
        public float Time { get; }
        public KeyCode Input { get; }

        public TimedInput(float time, KeyCode input)
        {
            Time = time;
            Input = input;
        }
    }
}