using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;

    /// <summary>
    /// Initializes Inventory item instance with variable values
    /// </summary>
    /// <param name="newItem"></param>
    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
    }
}
