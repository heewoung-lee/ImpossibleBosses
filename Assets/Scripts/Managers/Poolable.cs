
using UnityEngine;
[DisallowMultipleComponent]
public class Poolable : MonoBehaviour
{
    public bool IsUsing { get; set; }
    public bool WorldPositionStays { get; set; } = true;

    private string _poolablePath;
    public string PollablePath => _poolablePath;

    public void SetPoolableDirectory(string prefabPath)
    {
        _poolablePath = prefabPath;
    }

}