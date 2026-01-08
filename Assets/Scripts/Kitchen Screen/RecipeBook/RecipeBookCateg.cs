using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RecipeBookCateg: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fieldName;
    [SerializeField] private Button button;

    public void InitCategory (string name, Action action)
    {
       fieldName.text = name;
       button.onClick.AddListener(() => action()); 
       button.onClick.AddListener(RotateButton);
    }

    void RotateButton()
    {
        if (button.transform.localEulerAngles.x == 0)
            button.transform.localEulerAngles = new Vector3(180, 0, 0);
        else
            button.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}