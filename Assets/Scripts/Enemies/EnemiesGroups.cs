using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemiesGroups : MonoBehaviour
{
    [SerializeField] List<Enemies> enemies;

    private void Start()
    {
        foreach (var enemy in enemies)
        {
            enemy.Init();
        }
    }

    public Enemies GetPlayer()
    {
        return enemies.Where(x => x.HP > 0).FirstOrDefault();
    }
}
