using UnityEngine;

[CreateAssetMenu(
    fileName = "Data",
    menuName = "ScriptableObjects/TileScriptableObject",
    order = 1)]
public class TileScriptableObject : ScriptableObject
{
    [SerializeField] private Sprite blocked = null;
    [SerializeField] private Sprite empty = null;

    public Sprite Blocked => blocked;
    public Sprite Empty => empty;
}