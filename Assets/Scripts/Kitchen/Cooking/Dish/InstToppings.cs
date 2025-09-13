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
            InstNew();
            poolAvailable++;
        }

        Transform topping = toppingPool[poolMeter].transform;
        topping.position = GetMousePositionInWorldSpace();
        topping.SetParent(parent);

        //set 
        topping.GetComponent<DragAndDrop>().originalLocalPosition = topping.localPosition;
        topping.GetComponent<DragAndDrop>().parent = topping.parent;

        poolAvailable--;
        poolMeter++;
    }

    public void ReturnTopping()
    {

        Transform topping = toppingPool[poolMeter].transform;
        topping.localPosition = spawnPos;
        topping.SetParent(transform);

        //set 
        topping.GetComponent<DragAndDrop>().originalLocalPosition = transform.localPosition;
        topping.GetComponent<DragAndDrop>().parent = transform.parent;
        
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