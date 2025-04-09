using UnityEngine;
using Fusion;

public class Item : NetworkBehaviour, IStateAuthorityChanged
{
    public enum ItemType { Small, Big }
    public enum Weight { Light, Medium, Heavy }

    [Header("Settings")]
    public ItemType itemType;
    public Weight weight;
    public int priceLevel;

    [Header("Tutorial")]
    public bool tutorialboolSmallItem = false;
    public bool tutorialboolBigItem = false;

    [Header("Carry Settings")]
    public Vector3 carryOffset = new Vector3(0, 1, 1.5f); // tweak as needed

    [Networked] public NetworkBool IsCarried { get; set; }
    [Networked] public Vector3 NetworkedPosition { get; set; }
    [Networked] public Quaternion NetworkedRotation { get; set; }
    [Networked] public PlayerMovement Carrier { get; set; }

    private Vector3 _renderPosition;
    private Quaternion _renderRotation;
    private float _interpolationSpeed = 10f;

    private PlayerMovement _pendingPickupPlayer = null;

    public override void Spawned()
    {
        _renderPosition = transform.position;
        _renderRotation = transform.rotation;

        if (HasStateAuthority)
        {
            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;
        }
    }

    public void PickUp2(PlayerMovement player)
    {
        if (IsCarried) return;

        if (HasStateAuthority)
        {
            FinishPickup(player);
        }
        else
        {
            // Only request once
            if (_pendingPickupPlayer == null)
            {
                _pendingPickupPlayer = player;
                RPC_RequestPickup(player.Object.InputAuthority, player.Object);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestPickup(PlayerRef requestingPlayer, NetworkObject playerObject)
    {
        var player = playerObject.GetComponent<PlayerMovement>();
        if (player != null && !IsCarried)
        {
            if (Runner.GameMode == GameMode.Shared && Object.InputAuthority != requestingPlayer)
            {
                Object.RequestStateAuthority();
                _pendingPickupPlayer = player;
            }
            else
            {
                FinishPickup(player);
            }
        }
    }

    private void FinishPickup(PlayerMovement player)
    {
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // Maybe keep for small pickups
            tutorialboolSmallItem = true;
            return;
        }

        IsCarried = true;
        Carrier = player;

        if (HasStateAuthority)
        {
            UpdateCarriedPosition();
        }

        tutorialboolBigItem = true;
    }

    public void Drop2()
    {
        if (!IsCarried) return;

        IsCarried = false;

        if (HasStateAuthority && Carrier != null)
        {
            NetworkedPosition = Carrier.transform.position + Carrier.transform.forward * 1.5f;
            NetworkedRotation = Carrier.transform.rotation;
        }

        Carrier = null;
        _pendingPickupPlayer = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && IsCarried && Carrier != null)
        {
            UpdateCarriedPosition();
        }
    }

    private void UpdateCarriedPosition()
    {
        NetworkedPosition = Carrier.transform.position + Carrier.transform.TransformDirection(carryOffset);
        NetworkedRotation = Carrier.transform.rotation;
    }

    public override void Render()
    {
        if (IsCarried && Carrier != null)
        {
            if (Carrier.Object.HasInputAuthority)
            {
                transform.position = Carrier.transform.position + Carrier.transform.TransformDirection(carryOffset);
                transform.rotation = Carrier.transform.rotation;
                _renderPosition = transform.position;
                _renderRotation = transform.rotation;
            }
            else
            {
                _renderPosition = Vector3.Lerp(_renderPosition, NetworkedPosition, _interpolationSpeed * 2f * Time.deltaTime);
                _renderRotation = Quaternion.Slerp(_renderRotation, NetworkedRotation, _interpolationSpeed * 2f * Time.deltaTime);
                transform.SetPositionAndRotation(_renderPosition, _renderRotation);
            }
        }
        else
        {
            _renderPosition = Vector3.Lerp(_renderPosition, NetworkedPosition, _interpolationSpeed * Time.deltaTime);
            _renderRotation = Quaternion.Slerp(_renderRotation, NetworkedRotation, _interpolationSpeed * Time.deltaTime);
            transform.SetPositionAndRotation(_renderPosition, _renderRotation);
        }
    }

    public void StateAuthorityChanged()
    {
        if (HasStateAuthority)
        {
            if (_pendingPickupPlayer != null)
            {
                FinishPickup(_pendingPickupPlayer);
                _pendingPickupPlayer = null;
            }

            if (!IsCarried)
            {
                NetworkedPosition = transform.position;
                NetworkedRotation = transform.rotation;
            }
            else if (Carrier != null)
            {
                UpdateCarriedPosition();
            }
        }
    }
}
