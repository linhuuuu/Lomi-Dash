using UnityEditor;
using UnityEngine;
using PCG;
using System.Collections.Generic;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private List<bool> foldoutStates = new List<bool>(); // Add this
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Show other fields

        GameManager manager = (GameManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Generated Order Debug", EditorStyles.boldLabel);

        if (manager.orders == null)
        {
            EditorGUILayout.LabelField("No order generated");
            if (GUILayout.Button("Generate Order"))
            {
                manager.GenerateOrders(3, 2);
            }
        }
        else
        {
            while (foldoutStates.Count < manager.orders.Count)
                foldoutStates.Add(false);
            while (foldoutStates.Count > manager.orders.Count)
                foldoutStates.RemoveAt(foldoutStates.Count - 1);

            int count = 1;

            for (int i = 0; i < manager.orders.Count; i++)
            {
                var order = manager.orders[i];
                if (order != null)
                {
                    EditorGUILayout.LabelField("Order: " + count);

                    foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], "   Tree");
                    if (foldoutStates[i])
                    {
                        EditorGUILayout.LabelField("   Weight: " + order.weight.ToString("F1"));
                        PrintTree(order, "  ");
                    }
                    count++;
                }
            }

            if (GUI.changed)
                Repaint();
        }

        void PrintTree(OrderNode node, string indent)
        {
            EditorGUILayout.LabelField($"{indent}└─ {node.id} (w={node.weight:F1})");
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    if (child != null)
                        PrintTree(child, indent + "  ");
                }
            }
        }
    }
}



