using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyCfg", menuName = "SpaceInvaders/EnemyConfiguration")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public int points;
        public Color color;
        public GameObject prefab;
    }

    public List<EnemyType> enemyTypes;
}
