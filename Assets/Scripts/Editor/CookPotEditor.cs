using UnityEditor;
using UnityEngine;
using PCG;

[CustomEditor(typeof(CookPot))]
public class CookPotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Show other fields

        CookPot cookPot = (CookPot)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CookPot", EditorStyles.boldLabel);

        if (cookPot == null)
            EditorGUILayout.LabelField("Empty Pot Node");
        else
        {
            if (cookPot.boilNode != null)
            {
                EditorGUILayout.LabelField(cookPot.boilNode.id);
                EditorGUILayout.LabelField("  | Boil Time: " + cookPot.boilNode.time);
                EditorGUILayout.LabelField("  | Water Held: " + cookPot.boilNode.waterHeld);
            }


            if (cookPot.seasoningPotNode != null)
            {
                EditorGUILayout.LabelField(cookPot.seasoningPotNode.id);
                EditorGUILayout.LabelField("  | Salt Count: " + cookPot.seasoningPotNode.saltCount);
                EditorGUILayout.LabelField("  | Bawang Count: " + cookPot.seasoningPotNode.bawangCount);
                EditorGUILayout.LabelField("  | Pepper Count: " + cookPot.seasoningPotNode.pepperCount);  
            }

            if (cookPot.bonesNode != null)
            {
                EditorGUILayout.LabelField(cookPot.bonesNode.id);
                EditorGUILayout.LabelField("  | Bones Count: " + cookPot.bonesNode.count);  
            }


        }

        if (GUILayout.Button("Refresh"))
        {
            Repaint();
        }
    }

    void PrintTree(OrderNode node, string indent)
    {
        EditorGUILayout.LabelField($"{indent}└─ {node.id} (w={node.weight:F1})");
        if (Debug.isDebugBuild) Debug.Log("Added: " + node.id);
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




