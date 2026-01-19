#if UNITY_EDITOR
using System.Collections.Generic;
using PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrepTray))]
public class PrepTrayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrepTray prepTray = (PrepTray)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PrepTray", EditorStyles.boldLabel);

        if (prepTray.trayNode == null)
        {
            EditorGUILayout.LabelField("Empty Tray Node");
            return;
        }

        if (prepTray.trayNode.children != null)
        {
            if (prepTray.dishList != null)
            {
                EditorGUILayout.LabelField("Dishes");
                PrintArray(prepTray.dishList, " ");
            }

            if (prepTray.bevList != null)
            {
                EditorGUILayout.LabelField("Dishes");
                PrintArray(prepTray.bevList, " ");
            }

            if (prepTray.seasoningTray != null)
                EditorGUILayout.LabelField("Seasoning Tray Count: " + prepTray.seasoningTray.trayCount);
        }
        else
            PrintTree(prepTray.trayNode, " ");


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

    void PrintList(List<OrderNode> list, string indent)
    {
        if (list != null)
        {
            foreach (var child in list)
            {
                if (child != null)
                    PrintTree(child, indent + "  ");
            }
        }
    }

    void PrintArray(OrderNode[] list, string indent) {
        if (list != null)
        {
            foreach (var child in list)
            {
                if (child != null)
                    PrintTree(child, indent + "  ");
            }
        }
    }
}
#endif



