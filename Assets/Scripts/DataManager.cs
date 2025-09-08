using UnityEngine;

public class DataManager : MonoBehaviour
{
    public PlayerData playerData;
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