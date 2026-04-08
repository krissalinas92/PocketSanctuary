using UnityEngine;
using UnityEngine.SceneManagement;
public class SimpleSceneNavigation : MonoBehaviour
{
    public void GoToSanctuary()
    {
        SceneManager.LoadScene("SanctuaryScene");
    }
}
