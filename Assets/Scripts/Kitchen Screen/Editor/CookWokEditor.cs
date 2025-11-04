#if UNITY_EDITOR
using PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CookWok))]
public class CookWokEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CookWok cookWok = (CookWok)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CookWok", EditorStyles.boldLabel);

        if (cookWok.wokGroup == null)
            EditorGUILayout.LabelField("Empty Wok Node");
        else
        {
            if (cookWok.potGroup != null)
            {
                EditorGUILayout.LabelField(cookWok.potGroup.id);
                PrintTree(cookWok.potGroup, " ");
            }

            if (cookWok.sauteeNode != null)
            {
                EditorGUILayout.LabelField(cookWok.sauteeNode.id);
                EditorGUILayout.LabelField("  | Onion Count: " + cookWok.sauteeNode.onionCount);
                EditorGUILayout.LabelField("  | Bawang Count: " + cookWok.sauteeNode.bawangCount);
                EditorGUILayout.LabelField("  | Oil Count: " + cookWok.sauteeNode.oilCount);
                EditorGUILayout.LabelField("  | Onion Time: " + cookWok.sauteeNode.onionTime);
                EditorGUILayout.LabelField("  | Bawang Time: " + cookWok.sauteeNode.bawangTime);
                EditorGUILayout.LabelField("  | Oil Time: " + cookWok.sauteeNode.oilTime);
            }

            if (cookWok.soySauceNode != null)
            {
                EditorGUILayout.LabelField("  | SoySauce Count: " + cookWok.soySauceNode.count);
            }

            if (cookWok.noodlesNode != null)
            {
                EditorGUILayout.LabelField("  | Noodles Count: " + cookWok.noodlesNode.count);
                EditorGUILayout.LabelField("  | Noodles Time: " + cookWok.noodlesNode.time);
            }

            if (cookWok.eggNode != null)
            {
                EditorGUILayout.LabelField("  | Egg Count: " + cookWok.eggNode.count);
                EditorGUILayout.LabelField("  | Egg Mixed: " + cookWok.eggNode.isMixed);
            }

            if (cookWok.thickenerNode != null)
            {
                EditorGUILayout.LabelField("  | Thickener Count: " + cookWok.thickenerNode.count);
                EditorGUILayout.LabelField("  | Thickener Mixed: " + cookWok.eggNode.isMixed);
            }
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



