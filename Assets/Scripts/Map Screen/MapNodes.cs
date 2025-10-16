using UnityEngine;
using UnityEngine.UI;

public class MapNodes : MonoBehaviour
{
    [SerializeField] private RoundProfile profile;
    [SerializeField] private Button button;

     void Start()
    {
        button.onClick.AddListener(() => GameManager.instance.roundProfile = profile);
    }
}