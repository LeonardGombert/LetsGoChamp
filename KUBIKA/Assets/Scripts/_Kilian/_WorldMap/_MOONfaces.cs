using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kubika.Game
{
    public class _MOONfaces : MonoBehaviour
    {
        public ParticleSystem PS;
        public UnityEvent OnClick;

        public void ActivatePSFB()
        {
            Debug.Log("MOON FACES");
            PS.Play();
            StartCoroutine(TransitionTime());
        }

        IEnumerator TransitionTime()
        {
            yield return new WaitForSeconds(PS.main.duration);
            OnClick.Invoke();
        }
    }
}
