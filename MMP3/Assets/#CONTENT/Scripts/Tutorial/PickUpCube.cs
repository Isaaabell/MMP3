using UnityEngine;

public class PickUpCube : MonoBehaviour
{
    public bool isPickedUp = false; // Prüft, ob der Würfel bereits aufgehoben wurde

    void Update()
    {
        if (isPickedUp)
        {
            PickUp();
        }
    }

    // Wird aufgerufen, wenn der Spieler den Würfel aufhebt
    public void PickUp()
    {
        gameObject.SetActive(false); // Der Würfel verschwindet
    }
}
