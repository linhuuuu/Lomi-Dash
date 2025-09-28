using UnityEngine;
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
    public int orderID;

    //Patience
    public CustomerGroupTimer timer { set; get; }
    public CustomerTimerUI timerUIPrefab;
    public Transform timerloc { set; get; }

    private void Awake()
    {
        //Set References
        col = GetComponent<Collider>();
        mainCamera = CameraManager.cam.mainCam;
        tableLayer = 1 << 9; //tables are at layer 9

        //Timer Init and Subscribe
        timerloc = GameObject.Find("Content").transform;
        timer = GetComponent<CustomerGroupTimer>();
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
        if (table.occupied == true) { revertDefaults(); return;}

        //Attempt TrySitCustomers
        if (table.TrySitCustomers(this))
        {
            snapped = true;
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
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue, 0.2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tableLayer))
        {
            hitCollider = hit.collider;
        }

        if (hitCollider)
            if (Debug.isDebugBuild) Debug.Log(hitCollider.tag);

        col.enabled = true;
        return hitCollider != null;
    }

    private void revertDefaults() => transform.position = originalLocalPosition;
    
    #endregion
    #region Timer Coroutine
    public void StartCustomerTimer()
    {
        //Set Time
        foreach (Customer child in transform.GetComponentsInChildren<Customer>())
            timer.totalTime += child.patience;

        CustomerTimerUI ui = Instantiate(timerUIPrefab, timerloc);
        ui.SetTarget(timer, GetComponentInChildren<Customer>().portrait);

        StartCoroutine(timer.StartTimer());
    }

    public void CustomerLeave()
    {
        if (!snapped)
            RoundManager.roundManager.OnCustomerGroupLeaveStanding(this);
        else
            RoundManager.roundManager.OnCustomerGroupLeaveSitting(this);
        Destroy(gameObject);
    }

    private void Unsubscribe()
    {
        timer.OnTimerEnd -= CustomerLeave;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
