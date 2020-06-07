using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Kubika.CustomLevelEditor;
using Kubika.Saving;

namespace Kubika.Game
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager instance { get { return _instance; } }

        #region VARIABLE DECLARATIONS
        #region SCENE CANVASES
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas worldMapCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas transitionCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas hamburgerMenuCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas hamburgerMenuCanvas2;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas gameCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas levelPassedCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas levelEditorCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas customTestCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas winCanvas;
        #endregion

        #region WORLDMAP
        [FoldoutGroup("World Map")] [SerializeField] Text levelNameWM;
        [FoldoutGroup("World Map")] public Image topArrow; //used by WorldMap Rotation script
        [FoldoutGroup("World Map")] public Image bottomArrow; //used by WorldMap Rotation script

        #endregion         

        #region BURGER MENU
        [FoldoutGroup("Burger Menu")] [SerializeField] Button music;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite musicOn;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite musicOff;

        [FoldoutGroup("Burger Menu")] [SerializeField] Button sound;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite soundOn;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite soundOff;

        [FoldoutGroup("Burger Menu")] [SerializeField] GameObject hiddenMenuButtons;
        [FoldoutGroup("Burger Menu")] [SerializeField] GameObject openBurgerMenuButton;
        private bool soundIsOn = true, musicIsOn = true;
        #endregion

        #region IN GAME
        [FoldoutGroup("In Game")] [SerializeField] Image rightRotate, leftRotate;
        [FoldoutGroup("In Game")] [SerializeField] Button rightRotateButton, leftRotateButton;
        [FoldoutGroup("In Game")] [SerializeField] Sprite rightRotateOn, rightRotateOff;
        [FoldoutGroup("In Game")] [SerializeField] Sprite leftRotateOn, leftRotateOff;
        #endregion

        #region LEVEL EDITOR
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject levelEditorOptionsWindow;
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject levelEditorSaveWindow;
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject FunctionMode;
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject DecoratorMode;
        [FoldoutGroup("Level Editor")] public Dropdown playerLevelsDropdown;
        [FoldoutGroup("Level Editor")] public InputField saveLevelName;
        #endregion

        #region TRANSITION
        [FoldoutGroup("Fade Transition")] [SerializeField] Image fadeImage;
        [FoldoutGroup("Fade Transition")] [SerializeField] TransitionType transitionType;
        [FoldoutGroup("Fade Transition")] [SerializeField] float transitionDuration;
        [FoldoutGroup("Fade Transition")] [SerializeField] float timePassed;
        float startAlphaValue;
        float targetAlphaValue;
        bool gameDimmed = false;
        #endregion
        #endregion

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

            RefreshActiveScene();
        }

        #region REFRESH ACTIVE CANVASES
        public void RefreshActiveScene()
        {
            TurnOffAllCanvases();

            switch (ScenesManager.instance.currentActiveScene)
            {
                /*case ScenesIndex.MANAGER:
                    break;

                case ScenesIndex.USER_INTERFACE:
                    break;*/

                case ScenesIndex.TITLE_WORLD_MAP:
                    WorldMapPriority();
                    break;

                case ScenesIndex.GAME_SCENE:
                    GameCanvasPriority();
                    LevelsManager.instance.BakeLevels();
                    break;

                case ScenesIndex.WIN:
                    WinScreenSettings();
                    break;

                case ScenesIndex.LEVEL_EDITOR:
                    LevelEditorPriority();
                    FunctionModePriority();

                    ResetCurrentValues();
                    break;

                case ScenesIndex.CUSTOM_LEVELS:
                    LevelsManager.instance.RefreshUserLevels();
                    CustomLevelCanvasPriority();
                    break;

                case ScenesIndex.CREDITS:
                    break;

                default:
                    break;
            }
        }

        private void ResetCurrentValues()
        {
            SaveAndLoad.instance.currentOpenLevelName = "";
            SaveAndLoad.instance.currentKubicode = "";
            SaveAndLoad.instance.currentLevelLockRotate = false;
        }

        void ResetCanvasSortOrder()
        {
            worldMapCanvas.sortingOrder = 0;
            transitionCanvas.sortingOrder = 0;
            gameCanvas.sortingOrder = 0;
            winCanvas.sortingOrder = 0;
            levelEditorCanvas.sortingOrder = 0;
            levelPassedCanvas.sortingOrder = 0;
            customTestCanvas.sortingOrder = 0;
        }

        void TurnOffAllCanvases()
        {
            worldMapCanvas.enabled = false;
            //transitionCanvas.enabled = false;
            fadeImage.enabled = false;
            gameCanvas.enabled = false;
            winCanvas.enabled = false;
            levelEditorCanvas.enabled = false;
            hamburgerMenuCanvas.enabled = false;
            hamburgerMenuCanvas2.enabled = false;
            levelPassedCanvas.enabled = false;
            customTestCanvas.enabled = false;

            hiddenMenuButtons.SetActive(false);
            openBurgerMenuButton.SetActive(false);
            levelEditorOptionsWindow.SetActive(false);
            levelEditorSaveWindow.SetActive(false);
        }

        private void WorldMapPriority()
        {
            ResetCanvasSortOrder();
            if (worldMapCanvas != null) worldMapCanvas.enabled = true;
            worldMapCanvas.sortingOrder = 1000;

            topArrow.gameObject.SetActive(false);
            bottomArrow.gameObject.SetActive(false);
        }

        // called when camera is zoomed in
        public IEnumerator ZoomedWorldMapPriority()
        {
            ResetCanvasSortOrder();

            if (worldMapCanvas != null) worldMapCanvas.enabled = true;
            worldMapCanvas.sortingOrder = 1000;

            yield return new WaitForSeconds(1.5f);

            topArrow.gameObject.SetActive(true);
            bottomArrow.gameObject.SetActive(true);

            yield return null;
        }

        private void LevelEditorPriority()
        {
            ResetCanvasSortOrder();
            levelEditorCanvas.enabled = true;

            levelEditorOptionsWindow.SetActive(false);
            levelEditorSaveWindow.SetActive(false);

            levelEditorCanvas.sortingOrder = 1000;
        }

        //also caled from levels manager
        public void GameCanvasPriority()
        {
            ResetCanvasSortOrder();
            gameCanvas.enabled = true;

            levelPassedCanvas.sortingOrder = 1010;

            //reset burger enu params
            openBurgerMenuButton.SetActive(true);
            gameDimmed = false;

            if (hamburgerMenuCanvas != null) hamburgerMenuCanvas.enabled = true;
            if (hamburgerMenuCanvas2 != null) hamburgerMenuCanvas2.enabled = true;
            if (levelPassedCanvas != null) levelPassedCanvas.enabled = false;

            hiddenMenuButtons.SetActive(false);
            gameCanvas.sortingOrder = 1000;

            //Checking if the current level has ROtation enabled
            /*if (!_LoaderQueuer.instance._hasRotate) foreach (Button item in RotateButtons) item.gameObject.SetActive(false);
            else if (_LoaderQueuer.instance._hasRotate) foreach (Button item in RotateButtons) item.gameObject.SetActive(true);*/
        }

        public void CustomLevelCanvasPriority()
        {
            ResetCanvasSortOrder();

            gameCanvas.enabled = true;
            gameCanvas.sortingOrder = 1000;

            if (levelPassedCanvas != null) levelPassedCanvas.enabled = false;
            levelPassedCanvas.sortingOrder = 1010;

            customTestCanvas.enabled = true;
            customTestCanvas.sortingOrder = 1010;
        }

        private void WinScreenSettings()
        {
            ResetCanvasSortOrder();
            winCanvas.enabled = true;
            winCanvas.sortingOrder = 1000;
        }
        #endregion

        #region CHECK PLAYER INPUTS
        public void ButtonCallback(string button)
        {
            switch (button)
            {
                #region //GAME INPUTS
                case "GAME_RotateRight":
                    _KUBRotation.instance.RightTurn();
                    break;

                case "GAME_RotateLeft":
                    _KUBRotation.instance.LeftTurn();
                    break;

                case "GAME_Restart":
                    RefreshActiveScene();
                    LevelsManager.instance.RestartLevel();
                    break;

                case "GAME_Undo":
                    break;

                case "GAME_BurgerMenu":
                    StartCoroutine(DimGame());
                    break;
                #endregion

                #region //WORLDMAP INPUTS
                case "WORLDMAP_Play":
                    ScenesManager.instance._LoadScene(ScenesIndex.GAME_SCENE);
                    break;

                case "WORLDMAP_LevelEditor":
                    ScenesManager.instance._LoadScene(ScenesIndex.LEVEL_EDITOR);
                    break;

                case "WORLDMAP_UpArrow":
                    WorldmapManager.instance.currentBiome++;
                    WorldmapManager.instance.RefreshWorldArrowTargets();
                    _Planete.instance.AfterFace();
                    break;

                case "WORLDMAP_DownArrow":
                    WorldmapManager.instance.currentBiome--;
                    WorldmapManager.instance.RefreshWorldArrowTargets();
                    _Planete.instance.BeforeFace();
                    break;
                #endregion

                #region //BURGER MENU
                case "BURGER_Sound":
                    soundIsOn = !soundIsOn;
                    SwitchButtonSprite();
                    break;

                case "BURGER_Music":
                    musicIsOn = !musicIsOn;
                    SwitchButtonSprite();
                    break;

                case "BURGER_Close":
                    StartCoroutine(DimGame());
                    break;
                #endregion

                #region //LEVEL EDITOR
                case "LEVELEDITOR_isPlacing":
                    LevelEditor.instance.SwitchAction("isPlacing");
                    break;

                case "LEVELEDITOR_isDeleting":
                    LevelEditor.instance.SwitchAction("isDeleting");
                    break;

                case "LEVELEDITOR_isRotating":
                    LevelEditor.instance.SwitchAction("isRotating");
                    break;

                case "LEVELEDITOR_TestLevel":
                    LevelEditor.instance.SwitchAction(""); //turns off isRotating/Deleting/Placing bools
                    TestUserLevel();
                    break;

                case "LEVELEDITOR_SaveLevel":
                    UserSavedLevel();
                    break;

                case "LEVELEDITOR_SaveCurrentLevel":
                    UserSavedCurrentLevel();
                    break;

                case "LEVELEDITOR_NewLevel":
                    UserCreateNewLevel();
                    break;

                case "LEVELEDITOR_LoadLevel":
                    UserLoadLevel();
                    break;

                case "LEVELEDITOR_DeleteLevel":
                    UserDeleteLevel();
                    break;

                case "LEVELEDITOR_LeaveTestLevel":
                    ReturnToLevelEditor();
                    break;

                case "LEVELEDITOR_OptionsWindow":
                    OpenOptionsWindow();
                    break;

                case "LEVELEDITOR_SaveWindow":
                    OpenSaveWindow();
                    break;

                case "LEVELEDITOR_FXMode":
                    break;

                case "LEVELEDITOR_EmoteMode":
                    break;

                case "LEVELEDITOR_FunctionMode":
                    FunctionModePriority();
                    break;

                case "LEVELEDITOR_DecoratorMode":
                    DecoratorModePriority();
                    break;
                #endregion

                #region //GENERAL
                case "MAIN_MENU":
                    StartCoroutine(DimGame());
                    ScenesManager.instance._LoadScene(ScenesIndex.TITLE_WORLD_MAP);
                    break;

                case "TITLE_WORLDMAP":
                    ScenesManager.instance._LoadScene(ScenesIndex.TITLE_WORLD_MAP);
                    break;
                #endregion

                default:
                    break;
            }
        }
        #endregion

        #region GAME
        //called to turn sound on/off
        void SwitchButtonSprite()
        {
            if (musicIsOn == true) music.image.sprite = musicOn;
            if (musicIsOn == false) music.image.sprite = musicOff;

            if (soundIsOn == true) sound.image.sprite = soundOn;
            if (soundIsOn == false) sound.image.sprite = soundOff;
        }

        // called by rotator unlock
        public void TurnOnRotate()
        {
            rightRotate.sprite = rightRotateOn;
            leftRotate.sprite = leftRotateOn;

            rightRotateButton.enabled = true;
            leftRotateButton.enabled = true;
        }

        public void TurnOffRotate()
        {
            rightRotate.sprite = rightRotateOff;
            leftRotate.sprite = leftRotateOff;

            rightRotateButton.enabled = false;
            leftRotateButton.enabled = false;
        }

        //opens next level window
        public void OpenWinLevelWindow()
        {
            levelPassedCanvas.enabled = true;
        }

        //called on WinLeveWindow button press
        public void NextLevel()
        {
            levelPassedCanvas.enabled = false;
            LevelsManager.instance.ReturnToWorldMap();
        }
        #endregion

        #region WORLDMAP
        //called when the user selects a level from the worldmap
        public void UpdateWMInfo(string levelName)
        {
            levelNameWM.text = levelName;
        }
        #endregion

        #region LEVEL EDITOR
        //set function cubes as the active panel
        void FunctionModePriority()
        {
            LevelEditor.instance.currentCube = CubeTypes.MoveableCube; //optional, remove to let player pick Cube
            FunctionMode.SetActive(true);
            DecoratorMode.SetActive(false);
        }

        //set decorator cubes as the active panel
        void DecoratorModePriority()
        {
            LevelEditor.instance.currentCube = CubeTypes.FullStaticCube; //optional, remove to let player pick Cube
            DecoratorMode.SetActive(true);
            FunctionMode.SetActive(false);
        }

        //called by user when saving a level
        void UserSavedLevel()
        {
            SaveAndLoad.instance.UserSavingLevel(saveLevelName.text);
        }

        //called by user when saving a level
        void UserSavedCurrentLevel()
        {
            if (SaveAndLoad.instance.currentOpenLevelName != "") SaveAndLoad.instance.UserSavingCurrentLevel();
            else OpenSaveWindow();
        }

        //called by user when loading a level
        void UserLoadLevel()
        {
            //these dropdown caption texts are filled with the info provided by User Level Files
            SaveAndLoad.instance.UserLoadLevel(playerLevelsDropdown.captionText.text);
        }

        //called by user when loading a level
        void UserDeleteLevel()
        {
            //these dropdown caption texts are filled with the info provided by User Level Files
            SaveAndLoad.instance.UserDeleteLevel(playerLevelsDropdown.captionText.text);
        }

        private void UserCreateNewLevel()
        {
            _Grid.instance.RefreshGrid();
            SaveAndLoad.instance.currentOpenLevelName = "";
        }

        //called to open Level Editor options
        void OpenOptionsWindow()
        {
            levelEditorOptionsWindow.SetActive(!levelEditorOptionsWindow.activeInHierarchy);
        }

        //called to open Level Editor options
        void OpenSaveWindow()
        {
            levelEditorSaveWindow.SetActive(!levelEditorSaveWindow.activeInHierarchy);
        }


        // called when user tests level in Level Editor
        void TestUserLevel()
        {
            StartCoroutine(LevelsManager.instance.OpenTestLevel());

        }

        // called when user presses back button
        public void ReturnToLevelEditor()
        {
            StartCoroutine(LevelsManager.instance.CloseTestLevel());
        }
        #endregion

        #region FADING
        public void TransitionStart()
        {
            ResetCanvasSortOrder();
            fadeImage.enabled = true;
            transitionCanvas.sortingOrder = 9999;
        }

        public void TransitionOver()
        {
            transitionCanvas.sortingOrder = 0;
            transitionCanvas.enabled = false;
        }

        IEnumerator DimGame()
        {
            ResetCanvasSortOrder();

            if (gameDimmed == false)
            {
                fadeImage.enabled = true;
                openBurgerMenuButton.SetActive(false);
                hiddenMenuButtons.SetActive(true);

                timePassed = 0f;
                startAlphaValue = 0f;
                targetAlphaValue = .5f;

                StartCoroutine(FadeTransition(startAlphaValue, targetAlphaValue, transitionDuration, timePassed));

                hamburgerMenuCanvas.sortingOrder = 1000;
                hamburgerMenuCanvas2.sortingOrder = 1000;
                gameDimmed = true;
            }

            else if (gameDimmed == true)
            {
                timePassed = 0f;
                startAlphaValue = .5f;
                targetAlphaValue = 0f;
                StartCoroutine(FadeTransition(startAlphaValue, targetAlphaValue, transitionDuration, timePassed));
                gameDimmed = false;

                yield return new WaitForSeconds(transitionDuration);

                hiddenMenuButtons.SetActive(false);
                openBurgerMenuButton.SetActive(true);
                fadeImage.enabled = false;

                RefreshActiveScene();
            }
        }

        IEnumerator FadeTransition(float startValue, float targetValue, float transitionDuration, float timePassed)
        {
            transitionCanvas.sortingOrder = 999;
            float valueChange = targetValue - startValue;
            Color alphaColor = new Color();

            alphaColor = fadeImage.color;

            while (timePassed <= transitionDuration)
            {
                timePassed += Time.deltaTime;

                switch (transitionType)
                {
                    case TransitionType.LinearTween:
                        alphaColor.a = TweenManager.LinearTween(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                    case TransitionType.EaseInQuad:
                        alphaColor.a = TweenManager.EaseInQuad(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                    case TransitionType.EaseOutQuad:
                        alphaColor.a = TweenManager.EaseOutQuad(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                    case TransitionType.EaseInOutQuad:
                        alphaColor.a = TweenManager.EaseInOutQuad(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                    case TransitionType.EaseInOutQuint:
                        alphaColor.a = TweenManager.EaseInOutQuint(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                    case TransitionType.EaseInOutSine:
                        alphaColor.a = TweenManager.EaseInOutSine(timePassed, startValue, valueChange, transitionDuration);
                        yield return null;
                        break;
                }

                fadeImage.color = alphaColor;
            }
        }
        #endregion
    }
}
