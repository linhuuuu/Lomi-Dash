using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Scroller : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private float _x, _y;

    private Material _mat;

    void Awake()
    {
        _mat = Instantiate(_img.material);
        _img.material = _mat;
    }

    void Update()
    {
        if (_mat == null) return;

        Vector2 offset = _mat.mainTextureOffset;
        offset += new Vector2(_x, _y) * Time.deltaTime;
        _mat.mainTextureOffset = offset;
    }
}
