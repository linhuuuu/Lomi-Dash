using TMPro;
using UnityEngine;

public class InputFields : MonoBehaviour
{
    public TMP_InputField nameOutput;
    public TMP_InputField idOutput;
    public TextMeshProUGUI namefield;
    public TextMeshProUGUI idfield;

    public static InputFields inputFields;

    void Awake()
    {
        inputFields = this;
    }
    public void Add()
    {

        SaveSystem.save.data.UserName = nameOutput.text;
        SaveSystem.save.data.UserId = idOutput.text;
    }

    public void Reflect()
    {
        namefield.text = $"Name: {SaveSystem.save.data.UserName}";
        idfield.text = $"Current Account: {SaveSystem.save.data.UserId}";
    }
}
