using UnityEngine;
using UnityEngine.UI;
using LibGameAI.PRNGs;

public class DispersionGeneration : MonoBehaviour
{

    private RawImage image;
    private Color[] pixels;

    private void Awake()
    {
        //System.Random rnd = new System.Random();
        //System.Random rnd = new LCG();
        System.Random rnd = new XorShift128();

        image = GetComponent<RawImage>();
        pixels = new Color[Screen.width * Screen.height];
        for (int i = 0; i < Screen.height * Screen.width; i++)
            pixels[i] = Color.white;

        Texture2D text = new Texture2D(Screen.width, Screen.height);

        for (int i = 0; i < Screen.height * Screen.width / 5; i++)
        {
            int x = rnd.Next(Screen.width);
            int y = rnd.Next(Screen.height);
            pixels[y * Screen.width + x] = Color.black;
        }

        text.SetPixels(pixels);
        text.Apply();
        image.texture = text;

    }


    // // Start is called before the first frame update
    // private void Start()
    // {
    // }

    // // Update is called once per frame
    // private void Update()
    // {

    // }
}
