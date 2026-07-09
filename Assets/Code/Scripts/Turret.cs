using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject[] upgradePrefabs; // index 0 = level 2, index 1 = level 3
     

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float bps = 1f; // Bullets Per Second
    [SerializeField] private int baseUpgradeCost = 100;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float timeUntilFire;

    private int level = 1;
    private const int maxLevel = 3;
    private Plot plot;

    private void Awake()
    {
        bpsBase = bps;
        targetingRangeBase = targetingRange;
    }

    private void Start()
    {
        upgradeButton.onClick.AddListener(Upgrade);
    }

    public void SetPlot(Plot newPlot) // ADD THIS
    {
        plot = newPlot;
    }
    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
            return;
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firingPoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        bulletScript.SetTarget(target);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            targetingRange,
            (Vector2)transform.position,
            0f,
            enemyMask
        );

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(
            target.position.y - transform.position.y,
            target.position.x - transform.position.x
        ) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(
            new Vector3(0f, 0f, angle)
        );

        turretRotationPoint.rotation = Quaternion.RotateTowards(
            turretRotationPoint.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(
            transform.position,
            transform.forward,
            targetingRange
        );
    }
    #endif
    public void Upgrade()
    {
        if (level >= maxLevel) return;  

        if (CalculateCost() > Level.main.currency) return;

        Level.main.DecreaseCurrency(CalculateCost());

        level++;

        bps = CalculateBPS();
        targetingRange = CalculateRange();

        CloseUpgradeUI();
        if (level >= maxLevel)
        {
            upgradeButton.interactable = false;  // greys out the button
            // or: upgradeButton.gameObject.SetActive(false); to hide it completely
        }

        SpawnUpgradedTower();
        Debug.Log("New BPS: " + bps);
        Debug.Log("New Range: " + targetingRange);
        Debug.Log("New Cost: " + CalculateCost());  
    }

    private int CalculateCost()
    {
        return Mathf.RoundToInt(
            baseUpgradeCost * Mathf.Pow(level, 0.8f)
        );
    }
    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.6f);
    }

    private float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }
    private void SpawnUpgradedTower()
    {
        int prefabIndex = level - 2; // level 2 = index 0, level 3 = index 1

        if (upgradePrefabs != null && prefabIndex < upgradePrefabs.Length)
        {
            GameObject newTower = Instantiate(
                upgradePrefabs[prefabIndex],
                transform.position,
                transform.rotation
            );

            // carry over the current level so stats are correct
            Tower newTowerScript = newTower.GetComponent<Tower>();
            if (newTowerScript != null)
            {
                newTowerScript.SetLevel(level);
                newTowerScript.SetPlot(plot);         
                plot.SetTower(newTower, newTowerScript);
            }

            Destroy(gameObject);
        }
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
        bps = CalculateBPS();
        targetingRange = CalculateRange();
    }
}