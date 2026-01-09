using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RecipeBookCateg: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fieldName;
    [SerializeField] private Button dropDownButton;
    [SerializeField] private Button categButton;

    public void InitCategory (string name, Action action)
    {
       fieldName.text = name;
       categButton.onClick.AddListener(() => action()); 
       categButton.onClick.AddListener(RotateButton);
    }

    void RotateButton()
    {
        if (dropDownButton.transform.localEulerAngles.x == 0)
            dropDownButton.transform.localEulerAngles = new Vector3(180, 0, 0);
        else
            dropDownButton.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}