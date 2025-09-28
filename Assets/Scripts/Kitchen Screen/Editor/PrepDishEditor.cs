#if UNITY_EDITOR
using Codice.CM.SEIDInfo;
using PCG;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(PrepDish))]
public class PrepDishEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrepDish prepDish = (PrepDish)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("prepDish", EditorStyles.boldLabel);

        if (prepDish.dishNode == null)
            EditorGUILayout.LabelField("Empty Dish Node");
        else
        {
            if (prepDish.isLarge)
                EditorGUILayout.LabelField("Size: Large");
            else
                EditorGUILayout.LabelField("Size: Regular");

            if (prepDish.currentTopping != null)
                EditorGUILayout.LabelField("Current Topping:" + prepDish.currentTopping.id);

            if (prepDish.potGroup != null)
                {
                    EditorGUILayout.LabelField(prepDish.potGroup.id);
                    PrintTree(prepDish.potGroup, " ");
                }

            if (prepDish.wokGroup != null)
            {
                EditorGUILayout.LabelField(prepDish.wokGroup.id);
                PrintTree(prepDish.wokGroup, " ");
            }

            if (prepDish.toppingGroup != null)
            {
                EditorGUILayout.LabelField("Toppings");

                foreach (ToppingNode topping in prepDish.toppingGroup.children)
                {
                    EditorGUILayout.LabelField(" | " + topping.id + " x" + topping.count);
                }
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



