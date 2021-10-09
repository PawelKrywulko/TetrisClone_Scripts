using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    [SerializeField] private Sprite iconTrue;
    [SerializeField] private Sprite iconFalse;
    [SerializeField] private bool defaultIconState = false;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = defaultIconState ? iconTrue : iconFalse;
    }

    public void ToggleIcon(bool state)
    {
        if (!_image || !iconTrue || !iconFalse)
        {
            Debug.LogWarning("WARNING! Some icon is missing!");
            return;
        }

        _image.sprite = state ? iconTrue : iconFalse;
    }
}
