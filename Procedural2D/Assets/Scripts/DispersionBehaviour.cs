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

public class DispersionBehaviour : MonoBehaviour
{
    public enum RNG { System, Halton }

    [SerializeField]
    [Range(0.1f, 100f)]
    private float scale = 1;

    [SerializeField]
    private int numberOfPoints = 1000;

    [SerializeField]
    private RNG xRandomNumberGenerator = RNG.System;

    [SerializeField]
    private int xSeed = 2;

    [SerializeField]
    private RNG yRandomNumberGenerator = RNG.System;

    [SerializeField]
    private int ySeed = 3;

    private void Awake()
    {
        // Image width and height
        int width = (int)(Screen.width / scale);
        int height = (int)(Screen.height / scale);

        // Texture to show on screen, to be randomly created
        Texture2D texture = new Texture2D(width, height);

        // Get image component where to place the texture
        RawImage image = GetComponent<RawImage>();

        // Instantiate random number generator for x coordinate
        Random xRand = xRandomNumberGenerator == RNG.System
            ? new Random(xSeed)
            : new Halton(xSeed);

        // Instantiate random number generator for y coordinate
        Random yRand = yRandomNumberGenerator == RNG.System
            ? new Random(ySeed)
            : new Halton(ySeed);

        // Create a vector of pixels and set them all to white
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < height * width; i++)
            pixels[i] = Color.white;

        // Place the specified number of points
        for (int i = 0; i < numberOfPoints; i++)
        {
            int x = xRand.Next(width);
            int y = yRand.Next(height);
            pixels[y * width + x] = Color.black;
        }

        // Set and apply texture pixels
        texture.SetPixels(pixels);
        texture.Apply();

        // Place texture in image
        image.texture = texture;
    }

}
