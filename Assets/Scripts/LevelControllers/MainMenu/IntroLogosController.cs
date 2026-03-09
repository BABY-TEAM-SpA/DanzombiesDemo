using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLogosController : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
       
}
