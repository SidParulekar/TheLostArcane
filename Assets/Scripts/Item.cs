using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Collectible collectible;
    public string name;
    public Sprite image;
    public bool stackable = true;
    public float stat;
}
