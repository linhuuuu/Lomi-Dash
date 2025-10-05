using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
[System.Serializable]
public class PlayerSaveData
{
    [FirestoreProperty]
    public string userId { get; set; }

    [FirestoreProperty]
    public string playerName { get; set; }

    [FirestoreProperty]
    public string day { get; set; }

    [FirestoreProperty]
    public int happiness { get; set; }

    [FirestoreProperty]
    public int money { get; set; }

    [FirestoreProperty]
    public bool largeBowlUnlocked { get; set; } = false;

    [FirestoreProperty]
    public bool largeTrayUnlocked { get; set; } = false;

    [FirestoreProperty]
    public List<string> unlockedRecipeIds { get; set; } = new List<string>();
    
    [FirestoreProperty]
    public List<string> unlockedBeverageIds { get; set; } = new List<string>();

    [FirestoreProperty]
    public List<string> unlockedCustomerIds { get; set; } = new List<string>();

    [FirestoreProperty]
    public List<string> unlockedToppingIds { get; set; } = new List<string>();

    [FirestoreProperty]
    public List<string> unlockedStages { get; set; } = new List<string>();

}