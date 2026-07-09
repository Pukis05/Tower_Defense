using UnityEngine;
using UnityEngine.SceneManagement;


public class Level : MonoBehaviour
{
    public static Level main;

    public Transform startPoint;
    public Transform[] path;

    public int currency;
    public int lives = 3;


    private void Awake(){
        main = this;
    }

    private void Start()
    {
        currency = 100;
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool DecreaseCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough currency to buy turret!");
            return false;
        }

    }
    public void RemoveLife()
    {
        lives--;

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOver"); // replace with your scene name
    }
}
