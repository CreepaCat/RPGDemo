using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGroup : MonoBehaviour
{
    public UnityEvent OnGrounpCleared;
    List<Enemy> enemies = new();



    public void AddEnemy(Enemy enemy)
    {
        if (enemy == null || enemies.Contains(enemy)) return;
        enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
        {
            OnGrounpCleared?.Invoke();
        }
    }


}
