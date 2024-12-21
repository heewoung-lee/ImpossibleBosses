using System.Collections.Generic;
using UnityEngine;

public class PoolableTransformResetter : MonoBehaviour
{
    private List<Vector3> _originPositions;
    private List<Quaternion> _originRotations;

    private void Awake()
    {
        _originPositions = new List<Vector3>();
        _originRotations = new List<Quaternion>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _originPositions.Add(child.position);
            _originRotations.Add(child.rotation);
        }
    }
    private void OnDisable()
    {
        transform.position = Vector3.zero;

        for (int i = 0; i < _originPositions.Count; i++)
        {
            Transform child = transform.GetChild(i);
            child.position = _originPositions[i];
            child.rotation = _originRotations[i];
            child.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        
    }
}
