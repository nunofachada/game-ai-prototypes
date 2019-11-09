using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGUI : MonoBehaviour
{
    // Reference to the tile-based world
    private World world;

    // Start is called before the first frame update
    private void Awake()
    {
        world = GetComponent<World>();
    }

    // Draw a message when the goal is reached
    private void OnGUI()
    {
        // If goal is reached, draw window with restart button
        if (world.GoalReached)
        {
            // Place and draw restart window
            GUI.Window(0,
                new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100),
                DrawRestartWindow,
                "Goal reached!");
        }
    }

    // Draw window contents
    private void DrawRestartWindow(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {
            // Draw restart button
            if (GUI.Button(new Rect(50, 40, 100, 30), "Restart"))
            {
                // If button is clicked, restart
                world.Restart();
            }
        }
    }

}
