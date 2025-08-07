using SpacetimeDB.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class EntityController : MonoBehaviour
{
    const float LERP_DURATION_SEC = 0.1f;

    private static readonly int ShaderColorProperty = Shader.PropertyToID("_Color");

    [DoNotSerialize] public uint EntityId;

    protected float LerpTime;
    protected Vector3 LerpStartPosition;
    protected Quaternion LerpStartRotation;
    protected Vector3 LerpTargetPosition;
    protected Quaternion LerpTargetRotation;

    protected Vector3 targetVelocity;
    protected Vector3 targetAngularVelocity;

    protected Rigidbody rb;

    protected virtual void Spawn(uint entityId)
    {
        EntityId = entityId;

        var entity = GameManager.Conn.Db.Entity.EntityId.Find(entityId);
        LerpStartPosition = LerpTargetPosition = transform.position = (Vector3)entity.Position;
        LerpStartRotation = LerpTargetRotation = transform.rotation = new Quaternion(entity.Rotation.X, entity.Rotation.Y, entity.Rotation.Z, entity.Rotation.W);
        transform.localScale = Vector3.one;

        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnEntityUpdated(Entity newVal)
    {
        LerpTime = 0.0f;
        LerpStartPosition = transform.position;
        LerpStartRotation = transform.rotation;


        LerpTargetPosition = new Vector3(newVal.Position.X, newVal.Position.Y, newVal.Position.Z);
        LerpTargetRotation = new Quaternion(newVal.Rotation.X, newVal.Rotation.Y, newVal.Rotation.Z, newVal.Rotation.W);


        targetVelocity = newVal.Velocity;
        targetAngularVelocity = newVal.AngularVelocity;
    }

    public virtual void OnDelete(EventContext context)
    {
        Destroy(gameObject);
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        // Interpolate position and scale
        LerpTime = Mathf.Min(LerpTime + Time.deltaTime, LERP_DURATION_SEC);
        rb.MovePosition(Vector3.Lerp(LerpStartPosition, LerpTargetPosition, LerpTime / LERP_DURATION_SEC));
        rb.MoveRotation(Quaternion.Lerp(LerpStartRotation, LerpTargetRotation, LerpTime / LERP_DURATION_SEC));

        rb.velocity = targetVelocity;
        rb.angularVelocity = targetAngularVelocity;

    }

}