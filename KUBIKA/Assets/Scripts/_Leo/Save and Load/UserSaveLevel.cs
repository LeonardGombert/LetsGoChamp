using Kubika.Saving;
using UnityEngine;

namespace Kubika.Game
{
    public class UserSaveLevel : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UserSavedLevel()
        {
            SaveAndLoad.instance.UserSavingLevel(UIManager.instance.saveLevelName.text);
        }

        public void UserLoadLevel()
        {
            SaveAndLoad.instance.UserLoadLevel(UIManager.instance.playerLevelsDropdown.captionText.text);
        }
    }
}