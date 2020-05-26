using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kubika.Game
{

    [CreateAssetMenu(fileName = "_EmotePack", menuName = "_MaterialPacks/_EmotePack", order = 2)]
    public class _EmotePack : ScriptableObject
    {
        [Header("Base")]
        public Texture _BaseEmoteTex;

        [Header("Beton")]
        public Texture _BetonEmoteTex;

        [Header("Elevator")]
        public Texture _ElevatorEmoteTex;

        [Header("Counter")]
        public Texture _CounterEmoteTex;

        [Header("Rotators")]
        public Texture _RotatorsEmoteTex;

        [Header("Bomb")]
        public Texture _BombEmoteTex;

        [Header("Switch")]
        public Texture _SwitchEmoteTex;

        [Header("Ball")]
        public Texture _BallEmoteTex;

        [Header("Pastille")]
        public Texture _PastilleEmoteTex;

    }
}
