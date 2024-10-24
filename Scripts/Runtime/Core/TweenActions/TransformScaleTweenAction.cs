#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    // Created by Pablo Huaxteco
    [Serializable]
    public sealed class TransformScaleTweenAction : TweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Scale";

        [SerializeField]
        [Tooltip("If TRUE the input value will be used as a percentage (e.g. 50%, 100%, 200%...)")]
        private bool percentage;
        public bool Percentage
        {
            get => percentage;
            set => percentage = value;
        }

        [SerializeField]
        private Vector3 scale;
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        [SerializeField]
        private bool snapping;
        public bool Snapping
        {
            get => snapping;
            set => snapping = value;
        }

        private Transform targetTransform;
        private Vector3? originalScale;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            targetTransform = target.transform;
            originalScale = targetTransform.localScale;

            Vector3 endValue = percentage ? Vector3.Scale(originalScale.Value, scale / 100) : scale;
            TweenerCore<Vector3, Vector3, VectorOptions> tween = targetTransform.DOScale(endValue, duration).SetEase(ease);
            tween.SetOptions(axisConstraint, snapping);

            return tween;
        }

        public Vector3 GetEndValue(GameObject target)
        {
            Vector3 endValue = percentage ? Vector3.Scale(target.transform.localScale, scale / 100) : scale;
            if (relative)
                endValue += target.transform.localScale;

            return endValue;
        }

        public override void ResetToInitialState()
        {
            if (!originalScale.HasValue)
                return;

            targetTransform.localScale = originalScale.Value;
        }
    }
}
#endif