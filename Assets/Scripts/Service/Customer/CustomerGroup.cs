using UnityEngine;
public class CustomerGroup : MonoBehaviour
{
    //Group Component
    private Collider col;

    public Vector3 originalLocalPosition { set; get; }

    //Collider to be hit
    protected Collider hitCollider;
    private LayerMask tableLayer;
    public bool Snapped = false;

    //Camera
    protected Camera mainCamera;
    private float zOffset;

    //OrderReference
    public int orderID;

    //Patience
    private CustomerGroupTimer timer;
    public CustomerTimerUI timerUIPrefab;
    public Transform timerloc;

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
    }

    private void OnMouseDown()
    {
        if (Snapped) return;

        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
        originalLocalPosition = transform.position;

        foreach (CustomerInit child in transform.GetComponentsInChildren<CustomerInit>())
            child.PickUpCustomer();
    }

    private void OnMouseDrag()
    {
        if (Snapped) return;

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        //Validate
        if (Snapped)
        {
            return;
        }

        if (!RayCast())
        {
            revertDefaults();
            return;
        }

        //Check if Table Exists or Occupied
        if (!hitCollider.TryGetComponent(out TableDropZone table))
        {
            revertDefaults();
            return;
        }

        if (table.occupied == true)
        {
            revertDefaults();
            return;
        }

        if (table.TrySitCustomers(transform))
        {
            Snapped = true;

            //Sit Customer Sprite
            foreach (CustomerInit child in transform.GetComponentsInChildren<CustomerInit>())
                child.SitCustomer();

            //Timer ChargeBack
            StartCoroutine(timer.TimerAdd());
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

    private void revertDefaults()
    {
        // If no valid table, reset
        transform.position = originalLocalPosition;
        foreach (CustomerInit child in transform.GetComponentsInChildren<CustomerInit>())
            child.StandCustomer();
    }

    //Timer
    public void StartCustomerTimer()
    {
        //Set Time
        foreach (CustomerInit child in transform.GetComponentsInChildren<CustomerInit>())
            timer.totalTime += child.customerPatience;

        CustomerTimerUI ui = Instantiate(timerUIPrefab, timerloc);
        ui.SetTarget(timer, GetComponentInChildren<CustomerInit>().customerPortrait);

        StartCoroutine(timer.StartTimer());
    }

    public void CustomerLeave()
    {
        if (!Snapped)
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
}
