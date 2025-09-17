using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] public PlayerData playerData;  //Change to DataBase
    public static DataManager data;
    void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadFromFireBase(PlayerData data)
    {
        playerData = data;
        if (Debug.isDebugBuild) Debug.Log("Data Saved");
    }

    public void SaveToFireBase()
    {
        
    }


}