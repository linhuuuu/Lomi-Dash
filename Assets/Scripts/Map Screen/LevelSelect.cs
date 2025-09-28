using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private RoundProfile profile;
    [SerializeField] private Button button;

     void Start()
    {
        button.onClick.AddListener(() => GameManager.Instance.roundProfile = profile);
    }
}