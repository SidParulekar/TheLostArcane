using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image image;

    public Color selectedColor, notSelectedColor;    

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Select()
    {
        image.color = selectedColor;
        InventoryManager.Instance.ChangeSelectedSlot(this);
    }

    public void Unselect()
    {
        image.color = notSelectedColor;
    }
}
