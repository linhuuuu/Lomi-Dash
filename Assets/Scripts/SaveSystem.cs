using Firebase.Firestore;
using TMPro;
using UnityEngine;


public class SaveSystem : MonoBehaviour
{
    private FirebaseFirestore firestore;
    public SaveData data;
    public static SaveSystem save;

    public TMP_InputField input;
    public void Awake()
    {
        data = new SaveData();
        save = this;
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public async void SaveToCloud()
    {
        InputFields.inputFields.Add();
        await firestore.Document($"save_data/{data.UserId}").SetAsync(data);

    }

    public async void LoadFromCloud()
    {
        if (input.text == null)
        {
            if (Debug.isDebugBuild) Debug.Log($"No Value inputted!");
            return;
        }

        var snapshot = await firestore.Document($"save_data/{input.text}").GetSnapshotAsync();

        if (snapshot.Exists)
        {
            data = snapshot.ConvertTo<SaveData>();
            InputFields.inputFields.Reflect();
        }
        else
            if (Debug.isDebugBuild) Debug.Log($"Saving Failed. Please Try Again!");
    }

}
// [System.Serializable]
//     public class data
//     {
//         public string userName;
//         public int money;
//     }

// public class DataSaver : MonoBehaviour
// {

//     public data dts;
//     public string userID;
//     DatabaseReference dbRef;

//     private void Awake()
//     {
//         dbRef = FirebaseDatabase.DefaultInstance.RootReference;
//     }

//     public void SaveDataFn()
//     {
//         string json = JsonUtility.ToJson(dts);
//         dbRef.Child("users").Child(userID).SetRawJsonValueAsync(json);

//     }


//     public void LoadDataFn()
//     {
//         StartCoroutine(LoadDataEnum());
//     }

//     IEnumerator LoadDataEnum()
//     {
//         var serverData = debRef.Child("users").Child(userID).GetValueAsync();
//         yield return new WaitUntil(predicate: () => serverData.IsCompleted);

//         DataSnapshot snapshot = serverData.Result;
//         string jsonData = snapshot.GetRawJsonValue();

//         if (Json != null)
//         {

//         }
//     }

// }
