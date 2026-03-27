using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "fish", menuName = "Scriptable Objects/fish")]
public class fish : ScriptableObject
{
    public string fishname;
    public GameObject top;
    public GameObject bottom;
}
