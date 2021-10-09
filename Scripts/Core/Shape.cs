using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] private bool canRotate = true;
    [SerializeField] private Vector3 queueOffset;

    private List<GameObject> _squareChilds = new List<GameObject>(4);
    private ParticlePlayer[] _glowSquareFx;
    private const string GlowSquareTag = "LandShapeFX";

    private void Start()
    {
        _glowSquareFx = GameObject
            .FindGameObjectsWithTag(GlowSquareTag)
            .Select(obj => obj.GetComponent<ParticlePlayer>())
            .ToArray();

        foreach (Transform square in transform)
        {
            _squareChilds.Add(square.gameObject);
        }
    }

    private void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveLeft()
    {
        Move(Vector3.left);
    }
    public void MoveRight()
    {
        Move(Vector3.right);
    }
    public void MoveDown()
    {
        Move(Vector3.down);
    }
    public void MoveUp()
    {
        Move(Vector3.up);
    }

    private void RotateRight()
    {
        if (!canRotate) return;
        transform.Rotate(0,0,-90);
    }

    private void RotateLeft()
    {
        if (!canRotate) return;
        transform.Rotate(0,0,90);
    }

    public void RotateClockwise(bool clockwise)
    {
        if (clockwise)
        {
            RotateRight();
        }
        else
        {
            RotateLeft();
        }
    }

    public Vector3 GetQueueOffset()
    {
        return queueOffset;
    }

    public void LandShapeFx()
    {
        if (_squareChilds.Count != _glowSquareFx.Length || _glowSquareFx.Any(glow => glow == null)) return;

        for (int i = 0; i < _glowSquareFx.Length; i++)
        {
            var squarePosition = _squareChilds[i].transform.position;
            squarePosition.z = -2f;
            _glowSquareFx[i].transform.position = squarePosition;
            _glowSquareFx[i].Play();
        }
    }
}
