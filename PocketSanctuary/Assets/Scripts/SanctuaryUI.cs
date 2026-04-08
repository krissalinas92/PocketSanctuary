using UnityEngine;
using UnityEngine.SceneManagement;
public class SanctuaryUI : MonoBehaviour
{
    public void OnCustomizationClicked()
    {
        SceneManager.LoadScene("CustomizationScene");
    }
    public void OnTasksRewardsClicked()
    {
        SceneManager.LoadScene("TasksRewardsScene");
    }
}
