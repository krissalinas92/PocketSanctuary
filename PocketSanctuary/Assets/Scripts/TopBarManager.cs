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

    private void Awake()
    {
        EnsureDropdownReference();
    }

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
        EnsureDropdownReference();

        if (settingsDropdown == null)
        {
            Debug.LogWarning("SettingsDropdown reference is missing on " + gameObject.name);
            return;
        }

        bool shouldShow = !settingsDropdown.activeSelf;
        settingsDropdown.SetActive(shouldShow);

        if (shouldShow)
        {
            settingsDropdown.transform.SetAsLastSibling();
            transform.SetAsLastSibling();
        }

        Debug.Log("Settings dropdown " + (shouldShow ? "opened" : "closed"));
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void GoToShop()
    {
        SceneManager.LoadScene("ShopScene", LoadSceneMode.Single);
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

    private void EnsureDropdownReference()
    {
        if (settingsDropdown != null)
        {
            return;
        }

        Transform dropdownTransform = FindChildByName(transform, "SettingsDropdown");
        if (dropdownTransform != null)
        {
            settingsDropdown = dropdownTransform.gameObject;
        }
    }

    private Transform FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform match = FindChildByName(child, childName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
