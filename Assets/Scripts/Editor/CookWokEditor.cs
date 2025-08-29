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

        if (cookWok.wokNode == null)
            EditorGUILayout.LabelField("Empty Wok Node");
        else
        {
            if (cookWok.potNode != null)
            {
                EditorGUILayout.LabelField(cookWok.potNode.id);
                PrintTree(cookWok.potNode, " ");
            }

            if (cookWok.sauteeNode != null)
            {
                EditorGUILayout.LabelField(cookWok.sauteeNode.id);
            }
        }

        if (GUILayout.Button("Refresh"))
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
#endif



