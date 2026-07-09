using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 2;
    [SerializeField] private int currencyReward = 50;
    
    private bool isDestroyed = false;

    public void TakeDamage(int dmg)
    {
        hitPoints -= dmg;

        if (hitPoints <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            EnemySpawner.onEnemyDestroy.Invoke();
            Level.main.IncreaseCurrency(currencyReward);
            Destroy(gameObject);
        }
    }
}
