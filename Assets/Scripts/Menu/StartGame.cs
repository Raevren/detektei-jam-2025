using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class StartGame : MonoBehaviour
    {
        public void StartGameScene()
        {
            SceneManager.LoadScene("Scenes/PIN_SCENE");
        }
    }
}