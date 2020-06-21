using UnityEngine;
using UnityEngine.UI;
using LibGameAI.RNG;

public class DispersionGeneration : MonoBehaviour
{

    private RawImage image;
    private Color[] pixels;

    private void Awake()
    {
        //System.Random rnd = new System.Random();
        //System.Random rnd = new LCG();
        System.Random rnd = new XorShift128();
        System.Random rnd1 = new Halton(3);
        System.Random rnd2 = new Halton(179);

        image = GetComponent<RawImage>();
        pixels = new Color[Screen.width * Screen.height];
        for (int i = 0; i < Screen.height * Screen.width; i++)
            pixels[i] = Color.white;

        Texture2D text = new Texture2D(Screen.width, Screen.height);

        for (int i = 0; i < Screen.height * Screen.width / 5; i++)
        {
            // int x = rnd.Next(Screen.width);
            // int y = rnd.Next(Screen.height);
            int x = rnd1.Next(Screen.width);
            int y = rnd2.Next(Screen.height);
            //if (i % 1000 == 0) Debug.Log($"{i}: {Halton.Sequence(2, i)}");
            pixels[y * Screen.width + x] = Color.black;
        }

        text.SetPixels(pixels);
        text.Apply();
        image.texture = text;

    }

}
