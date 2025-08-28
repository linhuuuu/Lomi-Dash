using PCG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrepDish : MonoBehaviour
{
    public DishNode dishNode;
    public ToppingSectionNode toppingSectionNode;
    public List<ToppingNode> toppingNodes = new List<ToppingNode>();

    public List<string> ToppingsUnlocked = new List<string>() { "Kikiam", "Bola-Bola" };

    public ToppingNode currentTopping = new ToppingNode("");
    public void CreateDish()
    {
        if (dishNode == null) dishNode = new DishNode("DISH");
        if (toppingSectionNode == null) toppingSectionNode = new ToppingSectionNode("TOPPING_SECTION");
    }

    public void PlaceTopping(string type)
    {
        if (type == currentTopping.id)
        {
            currentTopping.toppingCount++;
        }
        else
        {
            if (currentTopping.toppingCount > 0)
            {
                AddTopping(currentTopping);
            }
            currentTopping.id = type;
            currentTopping.toppingCount = 1;
        }
    }

    public void AddTopping(ToppingNode top)
    {
        if (!toppingNodes.Any(t => t.id == top.id))
        {
            toppingNodes.Add(new ToppingNode(top.id));
        }
        foreach (ToppingNode topping in toppingNodes)
        {
            if (topping.id == top.id)
            {
                topping.toppingCount += top.toppingCount;
            }
        }
    }


    // public void CreatePotNode()
    // {
    //     if (boilNode == null) boilNode = new BoilNode("BOIL"); 
    //     if (bonesNode == null) new BonesNode("BONES");
    //     if (seasoningPotNode == null) new SeasoningPotNode("SEASONING_POT");

    //     // Create the container node
    //     potNode = new PotNode("POT_NODE");
    //     potNode.children = new List<OrderNode>
    //     {
    //         // Add children
    //         boilNode,
    //         bonesNode,
    //         seasoningPotNode
    //     };

    //     // Print the full structure
    //     if (Debug.isDebugBuild) Debug.Log("=== Pot Node Created ===");
    //     PrintNode(potNode, "");
    //     if (Debug.isDebugBuild) Debug.Log("========================");
    // }

    //DEBUG
    // private void PrintNode(OrderNode node, string indent)
    // {
    //     // Print current node
    //     string nodeInfo = node switch
    //     {
    //        BoilNode b => $"[BoilNode] Water: {b.waterHeld}, Time: {b.time}s",
    //        BonesNode bn => $"[BooleanNode] {bn.id} = {bn.count}",
    //         _ => $"[{node.GetType().Name}] {node.id}"
    //     };

    //     if (Debug.isDebugBuild) Debug.Log($"{indent}├─ {nodeInfo}");

    //     // Recurse into children
    //     for (int i = 0; i < node.children.Count; i++)
    //     {
    //         string newIndent = indent + "│  ";
    //         if (i == node.children.Count - 1)
    //             newIndent = indent + "   ";

    //         PrintNode(node.children[i], newIndent);
    //     }
    // }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<OrderNode>(out OrderNode tray))
        {
            //Creates the PotNode and Passes it to the wok.
            
            if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
        }
    }
}