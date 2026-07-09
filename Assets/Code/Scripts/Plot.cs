using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject towerObj;
    private Tower turret;
    private MGTurret mg;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (UIManager.main.IsHoveringUI()) return;

        if (towerObj != null)
        {
            // only open upgrade UI if it's an upgradeable tower
            if (turret != null)
            {
                turret.OpenUpgradeUI();
            }
            return;
        }

        Towers towerToBuild = BuildManager.main.GetSelectedTower();

        if (towerToBuild.cost > Level.main.currency)
        {
            Debug.Log("You can't afford this tower");
            return;
        }

        Level.main.DecreaseCurrency(towerToBuild.cost);

        towerObj = Instantiate(
            towerToBuild.prefab,
            transform.position,
            Quaternion.identity
        );

        // try to get whichever tower type was placed
        turret = towerObj.GetComponent<Tower>();
        mg = towerObj.GetComponent<MGTurret>();

        // only upgradeable towers need a plot reference
        if (turret != null)
        {
            turret.SetPlot(this);
        }
        if (mg != null)
        {
            towerObj.transform.position += new Vector3(-0.5f, 0f, 0f);
        }
    }

    // called by Tower.cs after upgrading to keep references updated
    public void SetTower(GameObject newTowerObj, Tower newTurret)
    {
        towerObj = newTowerObj;
        turret = newTurret;
    }
}