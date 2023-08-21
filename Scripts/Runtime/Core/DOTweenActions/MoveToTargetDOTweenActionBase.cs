﻿#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class MoveToTargetDOTweenActionBase : MoveDOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Move To Transform Position";

        [SerializeField]
        private Transform target;
        public Transform Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        private bool useLocalPosition;
        public bool UseLocalPosition
        {
            get => useLocalPosition;
            set => useLocalPosition = value;
        } 

        protected override Vector3 GetPosition()
        {
            if (useLocalPosition)
                return target.localPosition;

            return target.position;
        }
    }
}
#endif