using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 spawnPosition = new Vector3(player.RawEncoded % 2 == 0 ? 2 : -2, 1, 0);
            Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
