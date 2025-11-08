using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField] private MusicTrack gameTrack;
        
        public void StartGameScene()
        {
            SoundSystem.Instance.PlayMusic(gameTrack);
            SoundSystem.Instance.PlayGenericSfx(CommonSfx.Submit);
            SceneManager.LoadScene("Scenes/PIN_SCENE");
        }
    }
}