using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class SaveData
{
    string userID;
    string userName;

    [FirestoreProperty]
    public string UserId
    {
        get => userID;
        set => userID = value;
    }
    [FirestoreProperty]
    public string UserName
    {
        get => userName;
        set => userName = value;
    }
}