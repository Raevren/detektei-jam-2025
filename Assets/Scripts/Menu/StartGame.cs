using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class StartGame : MonoBehaviour
    {
        public void StartGameScene()
        {
            SoundSystem.Instance.PlayGenericSfx(CommonSfx.Submit);
            SceneManager.LoadScene("Scenes/PIN_SCENE");
        }
    }
}