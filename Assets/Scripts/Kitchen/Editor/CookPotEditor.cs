#if UNITY_EDITOR
using PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CookPot))]
public class CookPotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CookPot cookPot = (CookPot)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CookPot", EditorStyles.boldLabel);

        if (cookPot == null)
            EditorGUILayout.LabelField("Empty Pot Node");
        if (cookPot.boilNode != null)
        {
            EditorGUILayout.LabelField(cookPot.boilNode.id);
            EditorGUILayout.LabelField("  | Boil Time: " + cookPot.boilNode.time);
            EditorGUILayout.LabelField("  | Water Held: " + cookPot.boilNode.count);
        }


        if (cookPot.seasoningNode != null)
        {
            EditorGUILayout.LabelField(cookPot.seasoningNode.id);
            EditorGUILayout.LabelField("  | Salt Count: " + cookPot.seasoningNode.saltCount);
            EditorGUILayout.LabelField("  | Bawang Count: " + cookPot.seasoningNode.bawangCount);
            EditorGUILayout.LabelField("  | Pepper Count: " + cookPot.seasoningNode.pepperCount);
        }

        if (cookPot.bonesNode != null)
        {
            EditorGUILayout.LabelField(cookPot.bonesNode.id);
            EditorGUILayout.LabelField("  | Bones Count: " + cookPot.bonesNode.count);
        }

        if (GUILayout.Button("Refresh"))
            Repaint();
    }

    void PrintTree(OrderNode node, string indent)
    {
        EditorGUILayout.LabelField(node.ToString());
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
#endif



