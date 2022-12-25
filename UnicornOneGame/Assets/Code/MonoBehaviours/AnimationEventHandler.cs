using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.MonoBehaviours
{
    public class AnimationEventHandler : MonoBehaviour
    {
        public bool HitFlag { get; private set; }

        void Start()
        {

        }

        void Update()
        {

        }


        public void Clean()
        {
            HitFlag = false;
        }

        public void FootR()
        {
            // Nothing to do here
        }

        public void FootL()
        {
            // Nothing to do here
        }

        public void Hit()
        {
            HitFlag = true;
        }
    }
}
