using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TableDropZone : MonoBehaviour
{
    public List<Chair> chairs;
    public bool occupied = false;

    public bool TrySitCustomers(CustomerGroup customerGroup)
    {
        //Check Length
        Customer[] customers = customerGroup.GetComponentsInChildren<Customer>();
        if (customers.Length > chairs.Count) return false;

        customerGroup.transform.SetParent(transform);
        customerGroup.tableDropZone = this;
        for (int i = 0; i < customers.Length; i++)
        {
            Customer cus = customers[i];
            Chair chair = chairs[i];

            Vector3 seatPos = chairs[i].transform.position;
            Vector3 offset = new Vector3(0f, 0f, -0.2f);

            //Adjust seatpos of stoolchair
            if (chair.tag == "StoolChair")
                offset += new Vector3(0f, 0.6f, 0f);

            //Rotate Sprite
            if (chair.orientation == Chair.chairOrientation.right)
                cus.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

            cus.transform.position = seatPos;
            cus.transform.localPosition += offset;
            cus.spriteRenderer.sortingOrder = chairs[i].sortingOrder;
            cus.SitCustomer();
        }

        occupied = true;
        return true;
    }
}
