using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class PerlinScenario : StochasticScenario
    {
        [SerializeField]
        private float pixelSize = 10f;

        [SerializeField]
        private List<Color> heightColors = new List<Color>()
        {
            new Color(56/255f, 132/255f, 207/255f),  // Ocean blue
            new Color(194/255f, 178/255f, 128/255f), // Sand
            new Color(72/255f, 111/255f, 56/255f),   // Grass
            new Color(114/255f, 74/255f, 56/255f),   // Mountain brown
            new Color(243/255f, 246/255f, 251/255f)  // Snowy white
        };

        public override void Generate(Color[] pixels, int width, int height)
        {
            base.Generate(pixels, width, height);

            float xDisp = (float)PRNG.NextDouble() * pixelSize * width;
            float yDisp = (float)PRNG.NextDouble() * pixelSize * height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float value = Mathf.PerlinNoise(
                        xDisp + pixelSize * i / (float)width,
                        yDisp + pixelSize * (float)j / height);

                    value = Mathf.Clamp(value, 0f, 0.999f);

                    Color color = heightColors[(int)(value * heightColors.Count)];

                    pixels[i * width + j] = color;
                }
            }
        }
    }
}
