using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Kubika.Game
{
    public enum DifferentSound
    {
        SelectUivers,
        SelectPlanete,
        SelectMoon,
        UnlockedLevelSound,
        PlayedLevelSound,
        GoldLevelSound,
        Explosion,
        Pop,
        Pastille,
        VictoryBase,
        VictoryGold,
        Rotate,
        Bouton,
        SwitchON,
        SwitchOFF,
        PlaceCube,
        Delete,
        RotateLE,
        BoutonLE
    }
    public enum DifferentSceneSound
    {
        WorldMap,
        InGame,
        LevelEditor
    }


    public class _AudioManager : MonoBehaviour
    {
        private static _AudioManager _instance;
        public static _AudioManager instance { get { return _instance; } }

        public DifferentSceneSound SoundScene;

        [Space]
        [Header("AUDIO MIXER")]
        public AudioMixerGroup audioMixer;

        [Space]
        [Header("BG MUSIC")]
        public AudioClip BGMusic;

        [Space]
        [Header("WORLD MAP SOUNDS")]
        public AudioClip SelectUivers;
        public AudioClip SelectPlanete;
        public AudioClip SelectMoon;
        public AudioClip UnlockedLevelSound;
        public AudioClip PlayedLevelSound;
        public AudioClip GoldLevelSound;

        [Space]
        [Header("IN GAME SOUNDS")]
        public AudioClip Explosion;
        public AudioClip Pop;
        public AudioClip Pastille;
        public AudioClip VictoryBase;
        public AudioClip VictoryGold;
        public AudioClip Rotate;
        public AudioClip Bouton;
        public AudioClip SwitchON;
        public AudioClip SwitchOFF;

        [Space]
        [Header("LEVEL EDITOR SOUNDS")]
        public AudioClip PlaceCube;
        public AudioClip Delete;
        public AudioClip RotateLE;
        public AudioClip BoutonLE;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
