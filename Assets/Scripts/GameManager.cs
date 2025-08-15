using PCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI phaseDisplayText;
    private Touch touch;
    private float timeTouchEnded;
    private float displayTime = .5f;


        void Start()
        {
            // Initialize RNG with a seed
            ProceduralRNG.Initialize(System.DateTime.Now.GetHashCode());

            // Generate a tray
            OrderNode order = OrderGenerator.GenerateTray(difficulty: 3);

            // Print it to console
            Debug.Log("🍽️ Generated Dish:");
            OrderGraphPrinter.PrintOrderGraph(order);
    }
}
