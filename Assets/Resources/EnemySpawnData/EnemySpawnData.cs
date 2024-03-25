using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObject/EnemySpawnData")]
public class EnemySpawnData : ScriptableObject
{
    public List<GameObject> EnemyList;
}
