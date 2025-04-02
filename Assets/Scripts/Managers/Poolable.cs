
using UnityEngine;
[DisallowMultipleComponent]
public class Poolable : MonoBehaviour
{
    public bool IsUsing { get; set; }
    public bool WorldPositionStays { get; set; } = true;

}