using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    //[Header("Scene Settings")]
    //public string nextSceneName = "SanctuaryScene";

    /* private void Start()
    {
        if (statusText != null)
        {
            statusText.text = "Welcome back to Pocket Sanctuary";
        }
    }
    */
    public void OnLoginPressed() 
    {
        SaveManager.EnsureLoaded();

        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter username and password";
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            statusText.text = "Please enter username";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter password";
            return;
        }

        statusText.text = "Logging in...";
        SaveManager.Data.username = username;
        SaveManager.Save();
        SceneManager.LoadScene("SanctuaryScene", LoadSceneMode.Single);
    }
}
