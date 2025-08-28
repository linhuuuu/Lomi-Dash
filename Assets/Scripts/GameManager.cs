using PCG;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI phaseDisplayText;
    private Touch touch;
    private float timeTouchEnded;

        void Start()
        {
            // Initialize RNG with a seed
            ProceduralRNG.Initialize(System.DateTime.Now.GetHashCode());

            // Generate a tray
            OrderNode order = OrderGenerator.GenerateTray(difficulty: 3);

        float score = order.Evaluate(order);
        // Print it to console
        if (Debug.isDebugBuild) Debug.Log("Generated Dish 1 :");
        OrderGraphPrinter.PrintOrderGraph(order);

        if (Debug.isDebugBuild) Debug.Log("Total Score: " + score);

    }
}
