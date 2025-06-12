using UnityEngine;

public class MoveTest : MonoBehaviour
{


    Vector3 _originPosition = Vector3.zero;
    Vector3 _targetVector = Vector3.zero;

    float elaspTime = 0f;

    private void Start()
    {
        _originPosition = transform.position;
        _targetVector = transform.position + new Vector3(5,0,5);
    }
    void Update()
    {
        elaspTime += Time.deltaTime;

        transform.position = Vector3.Lerp(_originPosition, _targetVector, elaspTime / 2);

        if (transform.position == _targetVector)
        {
            Vector3 tempVector = _originPosition;
            _originPosition = _targetVector;
            _targetVector = tempVector;
            elaspTime = 0;
        }
    }
}
