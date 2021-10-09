using UnityEngine;

public class Holder : MonoBehaviour
{
    public bool CanRelease { get; set; } = false;
    
    private Shape _heldShape = null;

    private const float HeldScale = 0.5f;

    public void Catch(Shape shape)
    {
        if (_heldShape)
        {
            Debug.LogWarning("Release a shape before trying to hold!");
            return;
        }
        if (!shape)
        {
            Debug.LogWarning("Invalid shape!");
            return;
        }

        _heldShape = shape;
        Transform shapeTransform = shape.transform;
        shapeTransform.position = transform.position + shape.GetQueueOffset();
        shapeTransform.rotation = Quaternion.identity;
        shapeTransform.localScale = Vector3.one * HeldScale;
    }

    public Shape Release()
    {
        _heldShape.transform.localScale = Vector3.one;
        var shape = _heldShape;
        _heldShape = null;
        CanRelease = false;
        return shape;
    }

    public bool IsEmpty()
    {
        return _heldShape == null;
    }
}
