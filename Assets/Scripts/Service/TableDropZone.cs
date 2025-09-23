using System.Collections.Generic;
using UnityEngine;

public class TableDropZone : MonoBehaviour
{
    public List<Chair> chairs;
    public bool occupied = false;
    public Vector3 offset = new Vector3(0f, 0.6f, 0f);
    public bool TrySitCustomers(Transform customerGroup)
    {
        //Update SpawnPoint probably put this in another script
        RoundManager.roundManager.OnCustomerGroupSit(customerGroup.GetComponent<CustomerGroup>());

        customerGroup.SetParent(transform);
        CustomerInit[] customers = customerGroup.GetComponentsInChildren<CustomerInit>();

        if (customers.Length > chairs.Count) return false;  //Check Length
        for (int i = 0; i < customers.Length; i++)
        {
            Vector3 seatPos = chairs[i].transform.position;
            Vector3 offset = new Vector3(0f, 0f, -0.2f);

            //Adjust seatpos of stoolchair
            if (chairs[i].tag == "StoolChair")
                offset += new Vector3(0f, 0.6f, 0f);

            //Rotate Sprite if needed
            if (chairs[i].orientation == Chair.chairOrientation.right)
                customers[i].transform.localEulerAngles = new Vector3(0f, 180f, 0f);

            customers[i].transform.position = seatPos;
            customers[i].transform.localPosition += offset;
            customers[i].spriteRenderer.sortingOrder = chairs[i].sortingOrder;

        }

        occupied = true;
        return true;
    }
}
