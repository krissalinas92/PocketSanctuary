using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TopBarManager : MonoBehaviour
{
  [Header("UI References")]
  public TMP_Text coinText;
  public GameObject settingsDropdown;

  [Header("Coin Settings")]
  public int currentCoins = 250;

  private void Start()
    {
        UpdateCoinDisplay();

        if (settingsDropdown != null)
        {
            settingsDropdown.SetActive(false);
        }
    }

    public void ToggleSettingsDropdown()
    {
        Debug.Log("Cog clicked");

    if (settingsDropdown == null)
    {
        Debug.Log("SettingsDropdown is NULL");
        return;
    }

        if (settingsDropdown != null)
        {
            settingsDropdown.SetActive(!settingsDropdown.activeSelf);
        }
    }
    public void GoToSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void Logout()
    {
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinDisplay();
    }    
    public void SetCoins(int amount)
        {
            currentCoins = amount;
            UpdateCoinDisplay();
        }
    private void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + currentCoins;
        }
    }
}
