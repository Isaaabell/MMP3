using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Small, Big }
    public enum Weight { Light, Medium, Heavy }

    public ItemType itemType;
    public Weight weight; //ToDo: Implement
    public int priceLevel; //ToDo: Implement

    private bool isCarried = false;
    private ItemDropZone dropZone = null;
    public bool tutorialboolSmallItem = false;
    public bool tutorialboolBigItem = false;

    public void PickUp(Transform player)
    {
        // Debug.Log("Picking up item: " + gameObject.name);
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // End Point Small items
            tutorialboolSmallItem = true;
        }
        else if (itemType == ItemType.Big)
        {
            isCarried = true;
            transform.SetParent(player);
            transform.localPosition = new Vector3(0, 1, 0); // ToDo: Adjust carrying position
        }
    }

    public void Drop()
    {
        if (!isCarried)
        {
            return;
        }

        // Debug.Log("Dropping item: " + gameObject.name);
        isCarried = false;
        transform.SetParent(null);
        gameObject.SetActive(true);
    }
}
