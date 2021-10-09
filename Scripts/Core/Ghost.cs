using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1f, 1f, 1f, 0.2f);
    
    private Shape _ghostShape;
    private bool _hitBottom = false;
    
    private readonly Vector3 _ghostZOffset = Vector3.forward;

    public void DrawGhost(Shape originalShape, Board gameBoard)
    {
        if (!_ghostShape)
        {
            _ghostShape = Instantiate(originalShape, originalShape.transform.position + _ghostZOffset, originalShape.transform.rotation);
            _ghostShape.gameObject.name = "GhostShape";

            SpriteRenderer[] allRenderers = _ghostShape.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in allRenderers)
            {
                spriteRenderer.color = color;
            }
        }
        else
        {
            _ghostShape.transform.position = originalShape.transform.position + _ghostZOffset;
            _ghostShape.transform.rotation = originalShape.transform.rotation;
            _ghostShape.transform.localScale = Vector3.one;
        }

        _hitBottom = false;
        while (!_hitBottom)
        {
            _ghostShape.MoveDown();
            if (!gameBoard.IsValidPosition(_ghostShape))
            {
                _ghostShape.MoveUp();
                _hitBottom = true;
            }
        }
    }

    public void ResetGhostShape()
    {
        Destroy(_ghostShape.gameObject);
    }
}
