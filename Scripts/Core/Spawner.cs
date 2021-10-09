using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Shape[] allShapes;
    [SerializeField] private Transform[] queuedShapeTransforms = new Transform[3];
    [SerializeField] private ParticlePlayer spawnFx;

    private const float QueueScale = 0.5f;
    
    private readonly Queue<Shape> _queuedShapes = new Queue<Shape>(3);

    private void Awake()
    {
        InitQueue();
    }

    private Shape GetRandomShape()
    {
        int i = Random.Range(0, allShapes.Length);
        return allShapes[i] ? allShapes[i] : null;
    }

    private void InitQueue()
    {
        foreach (var t in queuedShapeTransforms)
        {
            Shape shape = InstantiateShape();
            shape.transform.position = t.position;
            shape.transform.localScale = Vector3.one * QueueScale;
            _queuedShapes.Enqueue(shape);
        }
    }

    public Shape SpawnShape()
    {
        return GetQueuedShape();
    }

    private Shape GetQueuedShape()
    {
        var shape = _queuedShapes.Dequeue();
        shape.transform.position = transform.position;
        StartCoroutine(GrowShape(shape, transform.position, 0.25f));
        
        var shapeToQueue = InstantiateShape();
        shapeToQueue.transform.position = queuedShapeTransforms.Last().position;
        shapeToQueue.transform.localScale = Vector3.one * QueueScale;
        _queuedShapes.Enqueue(shapeToQueue);
        RefreshQueueView();
        
        if (spawnFx) spawnFx.Play();
        return shape;
    }

    private void RefreshQueueView()
    {
        int i = 0;
        foreach (Shape queuedShape in _queuedShapes)
        {
            queuedShape.transform.position = queuedShapeTransforms[i].position + queuedShape.GetQueueOffset();
            ++i;
        }
    }

    private Shape InstantiateShape()
    {
        return Instantiate(GetRandomShape(), transform.position, Quaternion.identity);
    }

    private IEnumerator GrowShape(Shape shape, Vector3 position, float growTime = 0.5f)
    {
        float size = 0f;
        growTime = Mathf.Clamp(growTime, 0.1f, 2f);
        float sizeDelta = Time.deltaTime / growTime;

        while (size < 1f)
        {
            shape.transform.localScale = new Vector3(size, size, size);
            size += sizeDelta;
            shape.transform.position = position;
            yield return null;
        }
        
        shape.transform.localScale = Vector3.one;
    }
}
