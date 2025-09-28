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



