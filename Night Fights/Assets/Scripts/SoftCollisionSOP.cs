using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Soft Collision SO Pool", fileName = "Soft Collision SO Pool")]
public class SoftCollisionSOP : ScriptableObject
{
    public List<SoftCollision> softCollisions = new List<SoftCollision>();

    private void Awake()
    {
        softCollisions.Clear();
    }
}
