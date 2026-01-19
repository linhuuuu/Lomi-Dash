using System;
using System.Collections.Generic;
using System.Data;
using Firebase.Firestore;

[FirestoreData]
[System.Serializable]
public class RoundResults
{
    [FirestoreProperty]
    public string userId { get; set; }

    [FirestoreProperty]
    public float score { get; set; }

    [FirestoreProperty]
    public int totalDishes { get; set; }

    [FirestoreProperty]
    public int dishesCleared { get; set; }

    [FirestoreProperty]
    public int happyCustomers { get; set; }

    [FirestoreProperty]
    public int unhappyCustomers { get; set; }

    [FirestoreProperty]
    public float earnedHappiness { get; set; }

    [FirestoreProperty]
    public float earnedMoney { get; set; }

    [FirestoreProperty]
    public DateTime clearDate { get; set; }

    [FirestoreProperty]
    public float clearTime { get; set; }

    [FirestoreProperty]
    public int starCount { get; set; }
}