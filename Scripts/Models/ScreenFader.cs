using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float startAlpha = 1f;
    [SerializeField] private float targetAlpha = 0f;
    [SerializeField] private float delay = 0;
    [SerializeField] private float timeToFade = 1f;

    private float _inc;
    private float _currentAlpha;
    private MaskableGraphic _maskableGraphic;
    private Color _originalColor;

    private void Awake()
    {
        _maskableGraphic = GetComponent<MaskableGraphic>();
        _originalColor = _maskableGraphic.color;
        _currentAlpha = startAlpha;
        _inc = ((targetAlpha - startAlpha) / timeToFade) * Time.deltaTime;
        Color tempColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _currentAlpha);
        _maskableGraphic.color = tempColor;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(delay);

        while (Mathf.Abs(targetAlpha - _currentAlpha) > 0.01f)
        {
            yield return null;
            _currentAlpha += _inc;
            Color tempColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _currentAlpha);
            _maskableGraphic.color = tempColor;
        }
    }
}
