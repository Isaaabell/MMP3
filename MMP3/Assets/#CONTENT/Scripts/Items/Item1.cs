using UnityEngine;


public class Item1 : MonoBehaviour
{
    public enum ItemType { Small, Big }
    public enum Weight { Light, Medium, Heavy }

    public ItemType itemType;
    public Weight weight; //ToDo: Implement
    public int priceLevel; //ToDo: Implement

    private ItemDropZone dropZone = null;
    public bool tutorialboolSmallItem = false;
    public bool tutorialboolBigItem = false;

    public bool IsCarried { get; private set; }

    public void PickUp(Transform player)
    {
        Debug.Log("Picking up item: " + gameObject.name);
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // End Point Small items
            tutorialboolSmallItem = true;
        }
        else if (itemType == ItemType.Big)
        {
            IsCarried = true;
            transform.SetParent(player);
            transform.localPosition = new Vector3(0, 1, 0f);
        }
    }


    public void Drop()
    {
        if (!IsCarried)
        {
            return;
        }

        Debug.Log("Dropping item: " + gameObject.name);

        IsCarried = false;
        transform.SetParent(null);

        // Update the networked position and rotation immediately
        gameObject.SetActive(true);
    }
}