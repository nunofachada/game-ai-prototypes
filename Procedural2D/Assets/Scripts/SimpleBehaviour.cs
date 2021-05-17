/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using UnityEngine.UI;
using LibGameAI.RNG;
using Random = System.Random;

public class SimpleBehaviour : MonoBehaviour
{
    public enum PRNG { System, XorShift128 }

    [SerializeField]
    [Range(0.1f, 100f)]
    private float scale = 1;

    [SerializeField]
    private PRNG randomNumberGenerator = PRNG.System;

    [SerializeField]
    private bool useSeed = false;

    [SerializeField]
    private int seed = 123;

    private void Awake()
    {
        // Random number generator
        Random rnd = null;

        // Image width and height
        int width = (int)(Screen.width / scale);
        int height = (int)(Screen.height / scale);

        // Create a vector of pixels
        Color[] pixels = new Color[width * height];

        // Texture to show on screen, to be randomly created
        Texture2D texture = new Texture2D(width, height);

        // Get image component where to place the texture
        RawImage image = GetComponent<RawImage>();

        // Instantiate the selected random number generator
        switch (randomNumberGenerator)
        {
            case PRNG.System:
                rnd = useSeed ? new Random(seed) : new Random();
                break;
            case PRNG.XorShift128:
                rnd = useSeed ? new XorShift128(seed) : new XorShift128();
                break;
        }

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
