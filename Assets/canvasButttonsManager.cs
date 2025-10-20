using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasButttonsManager : MonoBehaviour
{
    [SerializeField] private Button almanac;
    [SerializeField] private Button store;
    void Start()
    {
        GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();

        almanac.onClick.AddListener(() => gameManager.NextScene("Almanac Screen"));
        store.onClick.AddListener(() => gameManager.NextScene("Shop Screen"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
