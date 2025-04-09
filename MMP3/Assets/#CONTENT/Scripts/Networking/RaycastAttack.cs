using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;

    public PlayerMovement PlayerMovement;

    void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        Ray ray = PlayerMovement.playerCamera.ScreenPointToRay(Input.mousePosition);
        ray.origin += PlayerMovement.playerCamera.transform.forward;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);
            if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out var hit))
            {
                if (hit.transform.TryGetComponent<Item>(out var item))
                {
                    item.PickUp2(PlayerMovement);
                }
            }
        }
    }
}