using System.Collections.Generic;
using System.Data;
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
    public int icon
    { get; set; }

    [FirestoreProperty]
    public int day { get; set; }

    [FirestoreProperty]
    public float happiness { get; set; }

    [FirestoreProperty]
    public float money { get; set; }

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

    [FirestoreProperty] public List<string> unlockedLocationIds { get; set; } = new List<string>();

    [FirestoreProperty] public List<string> unlockedTermIds { get; set; } = new List<string>();

    [FirestoreProperty] public Dictionary<string, List<bool>> unlockedSpecialCustomerIds { get; set; } = new Dictionary<string, List<bool>>();

    [FirestoreProperty] public List<string> unlockedAchievementIds { get; set; } = new List<string>();
    
    //Dialogue
    [FirestoreProperty]
    public Dictionary<string, bool> dialogueFlags { get; set; } = new();

}