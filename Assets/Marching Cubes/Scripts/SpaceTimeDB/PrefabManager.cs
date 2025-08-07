using SpacetimeDB.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager Instance;

    public ShipController ShipPrefab;
    public PlayerServerController PlayerPrefab;
    public GameObject marker;

    public Canvas markerCanvas;


    private void Awake()
    {
        Instance = this;
    }

    public static ShipController SpawnShip(Ship ship, PlayerServerController owner)
    {


        var entityController = Instantiate(Instance.ShipPrefab);
        entityController.name = $"Circle - {ship.EntityId}";
        entityController.Spawn(ship, owner);
        owner.OnShipSpawned(entityController);

        if (owner.IsLocalPlayer)
            entityController.gameObject.SetActive(false);
        else
        {
            GameObject marker = Instantiate(Instance.marker, Instance.markerCanvas.transform);
            marker.GetComponent<EnemyMarker>().target = entityController.transform;
        }

        return entityController;
    }


    public static PlayerServerController SpawnPlayer(Player player)
    {

        if (player.Identity != GameManager.Conn.Identity) // OTHER player
        {
            var playerController = Instantiate(Instance.PlayerPrefab);
            playerController.name = $"PlayerController - {player.Name}";
            playerController.Initialize(player);

            playerController.gameObject.SetActive(false);

            return playerController;
        }
        else // THIS player
        {
            var playerController = Instantiate(Instance.PlayerPrefab);
            playerController.name = $"PlayerController - {player.Name}";
            playerController.Initialize(player);

            return playerController;
        }


    }
}