using PCG;
using UnityEngine;

//PURELY FOR DEBUGGING PURPOSES
public static class OrderGraphPrinter
{

    public static void PrintOrderGraph(OrderNode node, string indent = "")
    {
        // Print current node
        string nodeInfo = GetNodeInfo(node);
        Debug.Log($"{indent}└─ [{node.id}] {nodeInfo} (w={node.weight:F1})");

        // Recurse into children
        for (int i = 0; i < node.children.Count; i++)
        {
            string newIndent = indent + "  ";
            if (i == node.children.Count - 1)
            {
                newIndent = indent + "  "; // last child
            }
            else
            {
                newIndent = indent + "│  "; // not last
            }

            PrintOrderGraph(node.children[i], newIndent);
        }
    }

    private static string GetNodeInfo(OrderNode node)
    {
        if (node is ToppingNode t)
        {
            return $"= {t.toppingName} x{t.expectedCount}";
        }
        if (node is IntSubStep i)
        {
            return $"= {i.expectedValue}{(i.name == "BoilTime" ? "s" : "")}";
        }
        if (node is BooleanSubStep b)
        {
            return $"= {b.expectedValue}";
        }
        if (node is BeverageNode bev)
        {
            return $"= {bev.drinkName}";
        }
        if (node is SeasoningNode s)
        {
            return $"= {s.seasoningType}";
        }

        // Default for containers
        return "";
    }
}