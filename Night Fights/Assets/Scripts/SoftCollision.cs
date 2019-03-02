using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ByteSheep.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class SoftCollision : MonoBehaviour
{
    public SoftCollisionEvent OnCollisionEnter;
    public SoftCollisionEvent OnCollisionExit;
    public SoftCollisionEvent OnCollisionStay;

    [SerializeField] float maxRepulsionForce=5;
    [ValidateInput("IsGreaterThanZero", "Max Repulsion Range must be greater than zero!")] [SerializeField] float maxRepulsionRange=2;
    [Required] [SerializeField] SoftCollisionSOP softCollisionSOP = null;
    List<SoftCollision> softCollisionsInRange = new List<SoftCollision>();
    List<SoftCollision> oldSoftCollisionsInRange = new List<SoftCollision>();
    [Required] new Rigidbody2D rigidbody2D = null;
    Vector2 direction;

    private bool IsGreaterThanZero(float myFloat)
    {
        return myFloat > 0;
    }

    private void Awake()
    {
        Debug.Assert(softCollisionSOP != null, "Error: softCollisionSOP must not be NULL in " + this + "!");
    }

    private void OnEnable()
    {
        softCollisionSOP.softCollisions.Add(this);
    }

    private void OnDisable()
    {
        softCollisionSOP.softCollisions.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSoftCollisionsInRange();
        ApplyForces();
    }

    void ApplyForces()
    {
        foreach(SoftCollision softCollision in softCollisionsInRange)
        {
            direction = (transform.position - softCollision.transform.position).normalized;
            Vector2 force = direction * (maxRepulsionForce + softCollision.maxRepulsionForce);
            float distance = Vector2.Distance(softCollision.transform.position, transform.position);
            rigidbody2D.AddForce(Vector2.Lerp(Vector2.zero, force, 1 - (distance / maxRepulsionRange)));
        }
    }

    void UpdateSoftCollisionsInRange()
    {
        List<SoftCollision> swapList = oldSoftCollisionsInRange;
        oldSoftCollisionsInRange = softCollisionsInRange;
        softCollisionsInRange = swapList;
        foreach(SoftCollision softCollision in softCollisionSOP.softCollisions)
        {
            Debug.Assert(softCollision != null, "softCollision in " + softCollisionSOP.softCollisions + " is null!");
            if (softCollision != this && Vector2.Distance(softCollision.transform.position, transform.position) < (maxRepulsionRange + softCollision.maxRepulsionRange))
            {
                softCollisionsInRange.Add(softCollision);
                if (oldSoftCollisionsInRange.Contains(softCollision))
                {
                    OnCollisionStay.Invoke(softCollision);
                    oldSoftCollisionsInRange.Remove(softCollision);
                }
                else
                {
                    OnCollisionEnter.Invoke(softCollision);
                    oldSoftCollisionsInRange.Remove(softCollision);
                }
            }
        }
        foreach(SoftCollision softCollision in oldSoftCollisionsInRange)
        {
            OnCollisionExit.Invoke(softCollision);
            oldSoftCollisionsInRange.Remove(softCollision);
        }
    }
}

public class SoftCollisionEvent : QuickEvent<SoftCollision> {}