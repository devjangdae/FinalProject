using System;
using UnityEngine;

namespace Gamekit3D
{
    public class SceneControllerWrapper2 : MonoBehaviour
    {
        public void RestartZone(bool resetHealth)
        {
            SceneController2.RestartZone(resetHealth);
        }

        public void TransitionToScene(TransitionPoint transitionPoint)
        {
            SceneController2.TransitionToScene(transitionPoint);
        }

        public void RestartZoneWithDelay(float delay)
        {
            SceneController2.RestartZoneWithDelay(delay, false);
        }

        public void RestartZoneWithDelayAndHealthReset(float delay)
        {
            SceneController2.RestartZoneWithDelay(delay, true);
        }
    }
}