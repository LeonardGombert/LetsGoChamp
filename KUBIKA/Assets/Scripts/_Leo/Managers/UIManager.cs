using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Kubika.CustomLevelEditor;

namespace Kubika.Game
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager instance { get { return _instance; } }

        //Game Canvas
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas gameCanvas;
        [FoldoutGroup("In Game")] [SerializeField] Image rightRotate, leftRotate;
        [FoldoutGroup("In Game")] [SerializeField] Sprite rightRotateOn, rightRotateOff;
        [FoldoutGroup("In Game")] [SerializeField] Sprite leftRotateOn, leftRotateOff;

        //Burger Menu
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas hamburgerMenuCanvas;
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas hamburgerMenuCanvas2;

        [FoldoutGroup("Burger Menu")] [SerializeField] Button music;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite musicOn;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite musicOff;
        private bool musicIsOn = true;

        [FoldoutGroup("Burger Menu")] [SerializeField] Button sound;

        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite soundOn;
        [FoldoutGroup("Burger Menu")] [SerializeField] Sprite soundOff;
        private bool soundIsOn = true;

        //Transition Canvas
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas transitionCanvas;
        [FoldoutGroup("Fade Transition")] [SerializeField] Image fadeImage;

        [FoldoutGroup("Fade Transition")] [SerializeField] TransitionType transitionType;
        [FoldoutGroup("Burger Menu")] [SerializeField] GameObject hiddenMenuButtons;
        [FoldoutGroup("Burger Menu")] [SerializeField] GameObject openBurgerMenuButton;

        //Transition Tween
        [FoldoutGroup("Fade Transition")] [SerializeField] float transitionDuration;
        [FoldoutGroup("Fade Transition")] [SerializeField] float timePassed;
        float startAlphaValue;
        float targetAlphaValue;
        bool gameDimmed = false;

        //Win Canvas
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas winCanvas;

        //World Map Canvas
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas worldMapCanvas;

        //Level Editor
        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas levelEditorCanvas;


        [FoldoutGroup("Scene Canvases")] [SerializeField] Canvas levelPassedCanvas;

        [FoldoutGroup("Level Editor")] [SerializeField] GameObject optionsWindow;
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject FunctionMode;
        [FoldoutGroup("Level Editor")] [SerializeField] GameObject DecoratorMode;

        public Dropdown playerLevelsDropdown;
        public InputField saveLevelName;

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

            RefreshActiveScene();
        }

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
                    break;

                case ScenesIndex.CUSTOM_LEVELS:
                    break;

                case ScenesIndex.CREDITS:
                    break;

                default:
                    break;
            }
        }

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

        void ResetCanvasSortOrder()
        {
            worldMapCanvas.sortingOrder = 0;
            transitionCanvas.sortingOrder = 0;
            gameCanvas.sortingOrder = 0;
            winCanvas.sortingOrder = 0;
            levelEditorCanvas.sortingOrder = 0;
            levelPassedCanvas.sortingOrder = 0;
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
        }

        private void WorldMapPriority()
        {
            ResetCanvasSortOrder();
            if (worldMapCanvas != null) worldMapCanvas.enabled = true;
            worldMapCanvas.sortingOrder = 1000;
        }

        private void LevelEditorPriority()
        {
            ResetCanvasSortOrder();
            levelEditorCanvas.enabled = true;

            if (hamburgerMenuCanvas != null) hamburgerMenuCanvas.enabled = true;
            if (hamburgerMenuCanvas2 != null) hamburgerMenuCanvas2.enabled = true;

            hiddenMenuButtons.SetActive(false);
            optionsWindow.SetActive(false);

            levelEditorCanvas.sortingOrder = 1000;
        }

        private void GameCanvasPriority()
        {
            ResetCanvasSortOrder();
            gameCanvas.enabled = true;

            levelPassedCanvas.sortingOrder = 1010;

            if (hamburgerMenuCanvas != null) hamburgerMenuCanvas.enabled = true;
            if (hamburgerMenuCanvas2 != null) hamburgerMenuCanvas2.enabled = true;
            if (levelPassedCanvas != null) levelPassedCanvas.enabled = false;

            hiddenMenuButtons.SetActive(false);
            gameCanvas.sortingOrder = 1000;

            //Checking if the current level has ROtation enabled
            /*if (!_LoaderQueuer.instance._hasRotate) foreach (Button item in RotateButtons) item.gameObject.SetActive(false);
            else if (_LoaderQueuer.instance._hasRotate) foreach (Button item in RotateButtons) item.gameObject.SetActive(true);*/
        }

        private void WinScreenSettings()
        {
            ResetCanvasSortOrder();
            winCanvas.enabled = true;
            winCanvas.sortingOrder = 1000;
        }

        void FunctionModePriority()
        {
            LevelEditor.instance.currentCube = CubeTypes.MoveableCube; //optional, remove to let player pick Cube
            FunctionMode.SetActive(true);
            DecoratorMode.SetActive(false);
        }

        void DecoratorModePriority()
        {
            LevelEditor.instance.currentCube = CubeTypes.FullStaticCube; //optional, remove to let player pick Cube
            DecoratorMode.SetActive(true);
            FunctionMode.SetActive(false);
        }

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

                case "WORLDMAP_TurnRight":
                    break;

                case "WORLDMAP_TurnLeft":
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
                    LevelEditor.instance.SwitchAction("");
                    break;

                case "LEVELEDITOR_Options":
                    OpenOptionsWindow();
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

                case "MAIN_MENU":
                    StartCoroutine(DimGame());
                    ScenesManager.instance._LoadScene(ScenesIndex.TITLE_WORLD_MAP);
                    break;

                case "TITLE_WORLDMAP":
                    ScenesManager.instance._LoadScene(ScenesIndex.TITLE_WORLD_MAP);
                    break;

                default:
                    break;
            }
        }

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
        }

        public void TurnOffRotate()
        {
            rightRotate.sprite = rightRotateOff;
            leftRotate.sprite = leftRotateOff;
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
            LevelsManager.instance._LoadNextLevel();
        }

        void OpenOptionsWindow()
        {
            optionsWindow.SetActive(!optionsWindow.activeInHierarchy);
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

    }
}
