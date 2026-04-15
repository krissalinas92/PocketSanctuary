using UnityEngine;
using UnityEngine.SceneManagement;
public class SanctuaryUI : MonoBehaviour
{
    public void OnCustomizationClicked()
    {
        SceneManager.LoadScene("CustomizationScene", LoadSceneMode.Single);
    }
    public void OnTasksRewardsClicked()
    {
        SceneManager.LoadScene("TasksRewardsScene", LoadSceneMode.Single);
    }
    public void OnAnimalClicked()
    {
        SceneManager.LoadScene("AnimalDetail", LoadSceneMode.Single);
    }
}
