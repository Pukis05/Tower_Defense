using UnityEngine;
using TMPro;
public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] Animator anim;

    private bool isMenuOpen = true;

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        anim.SetBool("MenuOpen", isMenuOpen);
    }

    private void OnGUI()
    {
        currencyUI.text = Level.main.currency.ToString();
    }

    public void SelectTower(int towerIndex)
    {
        
    }
}
