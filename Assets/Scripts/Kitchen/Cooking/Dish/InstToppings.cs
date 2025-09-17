using System.Collections.Generic;
using UnityEngine;

public class InstToppings : DragAndDrop
{
    private List<GameObject> toppingPool;
    [SerializeField] private Topping topping;   //Get From Prefs
    [SerializeField] private GameObject objPrefab; //Get From Prefs
    [SerializeField] private bool available; //Get From Prefs
    [SerializeField] private Vector3 spawnPos = new Vector3(100f, 20f, 0f);
    private int poolAvailable;
    private int poolMeter;

    void Start()
    {
        objPrefab.GetComponent<SpriteRenderer>().sprite = topping.sprite;
        this.GetComponent<SpriteRenderer>().sprite = topping.sprite;

        toppingPool = new List<GameObject>();
        InstPool();
    }

    #region Pool
    private void InstPool()
    {
        if (!available) return;

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
        newObj.GetComponent<ToppingPoolObj>().section = this;
        toppingPool.Add(newObj);

    }

    private void UseTopping(Transform parent)
    {
        if (poolAvailable == 0)
           
        {
             Debug.Log(poolAvailable);
            InstNew();
            poolAvailable++;
        }

        Debug.Log(poolMeter + "" + toppingPool[poolMeter].transform);
        Transform topping = toppingPool[poolMeter].transform;
        topping.SetParent(parent);
        topping.position = GetMousePos();
        topping.localRotation= Quaternion.identity;

        //set 
        topping.GetComponent<DragAndDrop>().originalLocalPosition = topping.localPosition;
        topping.GetComponent<DragAndDrop>().parent = topping.parent;

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

    public void ReturnTopping()
    {
        Transform topping = toppingPool[poolMeter - 1].transform;
        topping.SetParent(this.transform);
        topping.localPosition = spawnPos;


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

            UseTopping(targetDish.toppingSection);

            targetDish.PlaceTopping(topping.toppingName);
            revertDefaults();
            return;
        }
    }

    #endregion
}