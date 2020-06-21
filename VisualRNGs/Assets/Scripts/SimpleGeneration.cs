using UnityEngine;
using UnityEngine.UI;
using LibGameAI.RNG;

public class SimpleGeneration : MonoBehaviour
{

    private RawImage image;
    private Color[] pixels;

    private void Awake()
    {
        //System.Random rnd = new System.Random();
        System.Random rnd = new LCG();
        //System.Random rnd = new XorShift128();

        image = GetComponent<RawImage>();
        pixels = new Color[Screen.width * Screen.height];
        Texture2D text = new Texture2D(Screen.width, Screen.height);

        for (int i = 0; i < Screen.height; i++)
        {
            for (int j = 0; j < Screen.width; j++)
            {
                double val = rnd.NextDouble();
                Color color = val < 0.5 ? Color.white : Color.black;
                pixels[i * Screen.width + j] = color;
            }
        }

        text.SetPixels(pixels);
        text.Apply();
        image.texture = text;

    }
}
