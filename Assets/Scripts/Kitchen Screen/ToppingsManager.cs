using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingsManager : MonoBehaviour
{
    [SerializeField] private List<InstToppings> toppings;
    [SerializeField] private List<Topping> tutorialToppings;
    private List<Topping> availableToppings = new();

    private int toppingCounter = 0;

    void Start()
    {
        if (DataManager.data.loaded == true)
        {
            availableToppings = InventoryManager.inv.playerRepo.ToppingRepo;
        }

        else
        {
            if (Debug.isDebugBuild) Debug.Log("Data Manager is not loaded. Loading Default");
            availableToppings.Add(InventoryManager.inv.gameRepo.ToppingRepo.Find(c => c.id == "LIVER"));
            availableToppings.Add(InventoryManager.inv.gameRepo.ToppingRepo.Find(c => c.id == "BOLA-BOLA"));
            availableToppings.Add(InventoryManager.inv.gameRepo.ToppingRepo.Find(c => c.id == "KIKIAM"));
        }

        if (GameManager.instance.state == GameManager.gameState.beforeDay)
            InitToppings(availableToppings);
        else if (GameManager.instance.state == GameManager.gameState.tutorial)
            InitToppings(tutorialToppings);
    }

    public void InitToppings(List<Topping> toppingList)
    {
        if (toppingList == null) return;
        foreach (Topping toppingData in toppingList)
        {
            toppings[toppingCounter].topping = toppingData;
            toppings[toppingCounter].InitTopping();
            toppingCounter++;
        }

        //Disable everything else;
        for (int i = toppingCounter; i < toppings.Count; i++)
        {
            if (i < 12)
                toppings[i].gameObject.SetActive(false);
            else
                toppings[i].parent.gameObject.SetActive(false);
        }

    }

}
