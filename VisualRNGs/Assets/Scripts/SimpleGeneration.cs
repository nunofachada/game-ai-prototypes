using UnityEngine;
using UnityEngine.UI;
using LibGameAI.RNG;
using Random = System.Random;

public class SimpleGeneration : MonoBehaviour
{
    public enum PRNG { System, /*LCG,*/ XorShift128 }

    [SerializeField]
    [Range(0.1f, 100f)]
    private float scale = 1;

    [SerializeField]
    private PRNG randomNumberGenerator = PRNG.System;

    [SerializeField]
    private bool useSeed = false;

    [SerializeField]
    private int seed = 123;

    private RawImage image;
    private Color[] pixels;

    private void Awake()
    {
        // Random number generator
        Random rnd = null;

        // Image width and height
        int width = (int)(Screen.width / scale);
        int height = (int)(Screen.height / scale);

        // Texture to present, to be randomly created
        Texture2D texture = new Texture2D(width, height);

        // Instantiate the selected random number generator
        switch (randomNumberGenerator)
        {
            case PRNG.System:
                rnd = useSeed ? new Random(seed) : new Random();
                break;
            // case PRNG.LCG:
            //     rnd = useSeed ? new LCG(seed) : new LCG();
            //     break;
            case PRNG.XorShift128:
                rnd = useSeed ? new XorShift128(seed) : new XorShift128();
                break;
        }

        // Get image component where to place the texture
        image = GetComponent<RawImage>();

        // Create a vector of pixels
        pixels = new Color[width * height];

        // Fill vector of pixels with random black or white pixels
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Get a random value between 0 and 1
                double val = rnd.NextDouble();

                // Determine color based on obtained random value
                Color color = val < 0.5 ? Color.white : Color.black;

                // Set color in pixels array
                pixels[i * width + j] = color;
            }
        }

        // Set and apply texture pixels
        texture.SetPixels(pixels);
        texture.Apply();

        // Place texture in image
        image.texture = texture;
    }
}
