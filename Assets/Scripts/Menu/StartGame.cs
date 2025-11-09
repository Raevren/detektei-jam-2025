using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class StartGame : MonoBehaviour
    {
        private Button button;
        private void Start()
        {
            button = GetComponent<Button>();
            button.enabled = false;
            StartCoroutine(PreloadLocalization());
        }
        
        IEnumerator PreloadLocalization()
        {
            yield return Addressables.InitializeAsync();
            yield return LocalizationSettings.InitializationOperation;
            
            button.enabled = true;
        }
        
        public void StartGameScene()
        {
            SoundSystem.Instance.PlayGenericSfx(CommonSfx.Submit);
            SceneManager.LoadScene("Scenes/PIN_SCENE");
        }

        public void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}