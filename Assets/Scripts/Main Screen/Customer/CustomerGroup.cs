using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using PCG;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class CustomerGroup : MonoBehaviour
{
    //Group Components
    private Collider col;
    public Vector3 originalLocalPosition { set; get; }
    public bool snapped { set; get; }

    //Collider To Be Hit
    private Collider hitCollider;
    private LayerMask tableLayer;

    //Camera
    private Camera mainCamera;
    private float zOffset;

    //OrderReference
    public int orderID { set; get; }
    public int mainCustomerIdx { set; get; }
    public List<Customer> customers { set; get; }
    public TableDropZone tableDropZone { set; get; }
    public CustomerSpawnPoint spawnPoint { set; get; }

    //Patience
    public CustomerGroupTimer timer { set; get; }

    //UI
    private UITray trayObj;
    public OrderPrompt prompt { set; get; }
    private OrderQueueObj orderQueueObj;
    private UIModal modal;
    private List<Sprite> portraits;

    private void Awake()
    {
        col = GetComponent<Collider>();
        mainCamera = CameraManager.cam.mainCam;
        tableLayer = 1 << 9; //tables are at layer 9
        timer = GetComponent<CustomerGroupTimer>();
        customers = new List<Customer>();
        portraits = new List<Sprite>();
        timer.OnTimerEnd += CustomerLeave;

        snapped = false;
    }

    #region Dragging
    private void OnMouseDown()
    {
        if (snapped) return;

        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
        originalLocalPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        if (snapped) return;

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        //Validate and Fire Raycast
        if (snapped) return;
        if (!RayCast()) { revertDefaults(); return; }

        //Check if Table Exists or Occupied
        if (!hitCollider.TryGetComponent(out TableDropZone table)) { revertDefaults(); return; }
        if (table.occupied == true) { revertDefaults(); return; }

        //Attempt TrySitCustomers
        if (table.TrySitCustomers(this))
        {
            snapped = true;
            tableDropZone.occupied = true;
            spawnPoint.occupied = false;

            CallPrompt();
            StartCoroutine(timer.TimerAdd()); //Timer ChargeBack
            return;
        }
        revertDefaults();
        return;
    }

    private bool RayCast()
    {
        col.enabled = false;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tableLayer))
            hitCollider = hit.collider;

        col.enabled = true;
        return hitCollider != null;
    }

    private void revertDefaults() => transform.position = originalLocalPosition;

    #endregion
    #region Inst Prompt, Tray, OrderQueues

    public void InstTray(GameObject trayObjPrefab, RoundManager.Order roundManagerOrder)
    {
        trayObj = GameObject.Instantiate(trayObjPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<UITray>();
        trayObj.GetComponent<UITray>().InitTray(roundManagerOrder.order);
    }

    public void InstPrompt(GameObject promptPrefab, Transform spawnLoc)
    {
        mainCustomerIdx = PCG.ProceduralRNG.Range(0, customers.Count - 1);

        prompt = Instantiate(promptPrefab, Vector3.zero, Quaternion.identity, spawnLoc.transform).GetComponent<OrderPrompt>();
        prompt.SetTarget(timer, orderID, trayObj);
    }

    public void InstQueueOrder(GameObject orderQueueObjPrefab, Transform spawnLoc)
    {
        orderQueueObj = Instantiate(orderQueueObjPrefab, Vector3.zero, Quaternion.identity, spawnLoc.transform).GetComponent<OrderQueueObj>();

        //Portraits
        foreach (Customer customer in customers)
            portraits.Add(customer.portrait);

        orderQueueObj.SetTarget(trayObj, portraits[mainCustomerIdx]);
    }

    public void InstModal(GameObject modalPrefab, RoundManager.Order roundManagerOrder, GameObject parent)
    {
        modal = Instantiate(modalPrefab, Vector3.zero, Quaternion.identity, parent.transform).GetComponent<UIModal>();
        modal.transform.localPosition = modalPrefab.transform.localPosition;
        modal.transform.localEulerAngles = Vector3.zero;
        modal.SetTarget(trayObj, parent, orderQueueObj.GetComponent<Button>(), portraits);
    }

    public void CallPrompt()
    {
        //Adjust Position of Prompt
        Transform promptPos = prompt.transform;
        promptPos.SetParent(RoundManager.roundManager.promptCanvas);
        promptPos.position = customers[mainCustomerIdx].transform.position;

        Vector3 promptLocalPos = promptPos.localPosition;
        promptPos.localPosition = new Vector3(promptLocalPos.x + 1.25f, promptLocalPos.y + 1.8f, 0f);
        promptPos.localEulerAngles = Vector3.zero;
    }

    public void CallOrderQueue()
    {
        Transform orderPos = orderQueueObj.transform;
        orderPos.SetParent(RoundManager.roundManager.orderQueue);

        orderPos.transform.localEulerAngles = Vector3.zero;
        orderPos.transform.localPosition = Vector3.zero;
        orderPos.transform.localScale = Vector3.one;
    }

    #endregion
    #region Customer

    public void CustomerLeave()
    {
        if (!snapped)
            RoundManager.roundManager.OnCustomerGroupLeaveStanding(this);
        else
            RoundManager.roundManager.OnCustomerGroupLeaveSitting(this);

        RemoveAll();
    }

    public void RemoveAll()
    {
        Destroy(orderQueueObj.gameObject);
        Destroy(prompt.gameObject);
        Destroy(gameObject);
    }

    private void Unsubscribe() => timer.OnTimerEnd -= CustomerLeave;
    private void OnDestroy() => Unsubscribe();

    #endregion
}
