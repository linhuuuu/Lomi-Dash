using System.Collections.Generic;
using UnityEngine;

public class InstToppings : DragAndDrop
{
    private List<ToppingPoolObj> toppingPool = new();
    public Topping topping { set; get; }
    [SerializeField] private GameObject objPrefab; //Get From Prefs
    private Vector3 spawnPos = new Vector3(100f, 20f, 0f);
    private int poolAvailable;
    private int poolMeter;

    public void InitTopping()
    {
        parent.GetComponent<SpriteRenderer>().sprite = topping.containerSprite;
        transform.localPosition = Vector3.zero;
        objPrefab.GetComponent<SpriteRenderer>().sprite = topping.sprite;
        this.GetComponent<SpriteRenderer>().sprite = topping.sprite;
        InstPool();
    }

    #region Pool
    private void InstPool()
    {
        for (int i = 0; i < 10; i++)
        {
            InstNew();
        }

        poolAvailable = toppingPool.Count;
        poolMeter = 0;
    }

    private void InstNew()
    {
        var newObj = Instantiate(objPrefab, spawnPos, Quaternion.identity, transform);
        var newToppingPoolObj = newObj.GetComponent<ToppingPoolObj>();
        newToppingPoolObj.section = this;
        this.name = topping.toppingName;
        toppingPool.Add(newToppingPoolObj);

    }

    private void UseTopping(PrepDish dish)
    {
        if (poolAvailable == 0)

        {
            Debug.Log(poolAvailable);
            InstNew();
            poolAvailable++;
        }

        Debug.Log(poolMeter + "" + toppingPool[poolMeter].transform);   //Topping has been destoryed ro something what in the actual hell
        ToppingPoolObj topping = toppingPool[poolMeter];
        topping.transform.SetParent(dish.toppingSection);
        topping.transform.position = GetMousePos();
        Vector3 localPos = topping.transform.localPosition;
        topping.transform.localPosition = new Vector3(localPos.x, localPos.y, -0.6f);
        topping.transform.localRotation = Quaternion.identity;
        topping.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        //Set Detail Prompt
        ActiveTopping activeTopping = topping.GetComponent<ActiveTopping>();
        activeTopping.SetPanel(dish.toppingObjDetail);
        activeTopping.InitActiveTopping();

        //set 
        topping.GetComponent<DragAndDrop>().originalLocalPosition = topping.transform.localPosition;
        topping.GetComponent<DragAndDrop>().parent = dish.toppingSection;

        poolAvailable--;
        poolMeter++;
        
    }

    private Vector3 GetMousePos()
    {
        // Define the plane where interactions happen (e.g., counter top)
        Plane interactionPlane = new Plane(Vector3.up, new Vector3(0, -18f, 0)); // Y = -18

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (interactionPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return point;
        }

        // Fallback: use far point along ray
        return ray.GetPoint(1000f);
    }

    public void ReturnTopping(ToppingPoolObj topping)
    {
        topping.GetComponent<ActiveTopping>().RemovePanel();
        topping.transform.SetParent(this.transform);
        topping.transform.localPosition = spawnPos;

        //set 
        topping.GetComponent<DragAndDrop>().originalLocalPosition = spawnPos;
        topping.GetComponent<DragAndDrop>().parent = this.transform;

        poolAvailable++;
        poolMeter--;
    }
    #endregion

    #region Dropping

    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out PrepDish targetDish))
        {
            UseTopping(targetDish);
            targetDish.PlaceTopping(topping.toppingName);
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Topping")
        {
            Debug.Log("RUn");
            if (!hitCollider.TryGetComponent(out ToppingPoolObj top)) return;

            if (top.transform.parent.parent.TryGetComponent(out PrepDish dish))
            {
                UseTopping(dish);
                dish.PlaceTopping(topping.toppingName);
            }
            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

    #endregion
}