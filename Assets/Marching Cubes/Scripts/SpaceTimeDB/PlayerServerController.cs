using System.Collections.Generic;
using System.Linq;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class PlayerServerController : MonoBehaviour
{
    const int SEND_UPDATES_PER_SEC = 20;
    const float SEND_UPDATES_FREQUENCY = 1f / SEND_UPDATES_PER_SEC;

    public static PlayerServerController Local { get; private set; }

    private uint PlayerId;
    private float LastMovementSendTimestamp;
    private Vector2? LockInputPosition;
    private List<ShipController> OwnedShips = new List<ShipController>();

    public string Username => GameManager.Conn.Db.Player.PlayerId.Find(PlayerId).Name;
    public int NumberOfOwnedShips => OwnedShips.Count;
    public bool IsLocalPlayer => this == Local;

    [SerializeField] GameObject playerObject;
    Rigidbody playerRB;
    public void Initialize(Player player)
    {
        PlayerId = player.PlayerId;
        if (player.Identity == GameManager.LocalIdentity)
        {
            Local = this;
        }


        playerRB = playerObject.GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        // If we have any ships, destroy them
        foreach (var ship in OwnedShips)
        {
            if (ship != null)
            {
                Destroy(ship.gameObject);
            }
        }
        OwnedShips.Clear();
    }

    public void OnShipSpawned(ShipController ship)
    {
        OwnedShips.Add(ship);
    }

    public void OnShipDeleted(ShipController deletedShip)
    {
        // This means we got eaten
        if (OwnedShips.Remove(deletedShip) && IsLocalPlayer && OwnedShips.Count == 0)
        {
            // DeathScreen.Instance.SetVisible(true);
        }
    }


    /*
    private void OnGUI()
    {
        if (!IsLocalPlayer || !GameManager.IsConnected())
        {
            return;
        }

        GUI.Label(new Rect(0, 0, 100, 50), $"Total Mass: {TotalMass()}");
    }
    */

    //Automated testing members
    private bool testInputEnabled;
    private Vector3 testInput;

    public void SetTestInput(Vector2 input) => testInput = input;
    public void EnableTestInput() => testInputEnabled = true;

    public void Update()
    {
        Vector3 position = playerObject.transform.position;
        Vector3 velocity = playerRB.velocity;
        Vector3 angularVelocity = playerRB.angularVelocity;

        DbQuaternion rotation = new DbQuaternion(playerObject.transform.rotation.x, playerObject.transform.rotation.y, playerObject.transform.rotation.z, playerObject.transform.rotation.w);

        if (testInputEnabled) { position = testInput; }

        GameManager.Conn.Reducers.UpdatePlayerInput(position, rotation, velocity, angularVelocity);
    }
}
