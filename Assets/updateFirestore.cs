using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UpdateFirestore : MonoBehaviour
{
    async void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        
        await AddFieldToAllDocuments();
    }
    public async Task AddFieldToAllDocuments()
    {
        CollectionReference collection = FirebaseFirestore.DefaultInstance.Collection("players");

        // 1. Get all documents
        QuerySnapshot snapshot = await collection.GetSnapshotAsync();

        // 2. Prepare batched writes (more efficient than individual updates)
        WriteBatch batch = FirebaseFirestore.DefaultInstance.StartBatch();
        int updateCount = 0;

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {

            batch.Update(doc.Reference, "unlockedSpecialCustomerIds", new Dictionary<string, List<bool>>
            {
                {"JUAN", new List<bool> {false, false, false}},
            });


            // Firestore batch max: 500 ops
            if (updateCount % 500 == 0)
            {
                await batch.CommitAsync();
                batch = FirebaseFirestore.DefaultInstance.StartBatch();
            }
        }

        // Commit remaining
        if (updateCount % 500 != 0)
        {
            await batch.CommitAsync();
        }

        Debug.Log($"Added field to {updateCount} documents.");
    }
}
