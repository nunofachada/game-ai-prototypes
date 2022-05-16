namespace AIUnityExample.NGramsFight
{
    public struct TimedInput
    {
        public float Time { get; }
        public string Input { get; }

        public TimedInput(float time, string input)
        {
            Time = time;
            Input = input;
        }
    }
}