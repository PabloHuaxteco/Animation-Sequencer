#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    // Created by Pablo Huaxteco
    [Serializable]
    public sealed class RectTransformSizeDeltaTweenAction : TweenActionBase
    {
        public override Type TargetComponentType => typeof(RectTransform);
        public override string DisplayName => "Size Delta";

        [SerializeField]
        [Tooltip("If TRUE the input value will be used as a percentage (e.g. 50%, 100%, 200%...)")]
        private bool percentage;
        public bool Percentage
        {
            get => percentage;
            set => percentage = value;
        }

        [SerializeField]
        private Vector2 sizeDelta;
        public Vector2 SizeDelta
        {
            get => sizeDelta;
            set => sizeDelta = value;
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

        private RectTransform targetRectTransform;
        private Vector2 originalSize;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetRectTransform == null || targetRectTransform.gameObject != target)
            {
                targetRectTransform = target.transform as RectTransform;

                if (targetRectTransform == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component.");
                    return null;
                }
            }

            originalSize = targetRectTransform.sizeDelta;

            Vector2 endValue = percentage ? Vector2.Scale(targetRectTransform.rect.size, sizeDelta / 100) : sizeDelta;
            if (!relative && IsRectTransformStretched(targetRectTransform, out bool isHorizontallyStretched, out bool isVerticallyStretched))
            {
                Vector2 strechValue = -(targetRectTransform.rect.size - (endValue + targetRectTransform.sizeDelta));
                if (isHorizontallyStretched) endValue.x = strechValue.x;
                if (isVerticallyStretched) endValue.y = strechValue.y;
            }

            var tween = targetRectTransform.DOSizeDelta(endValue, duration);
            tween.SetOptions(axisConstraint, snapping);

            return tween;
        }

        public Vector2 GetEndValue(GameObject target)
        {
            RectTransform rectTransform = target.transform as RectTransform;
            Vector2 endValue = percentage ? Vector2.Scale(rectTransform.rect.size, sizeDelta / 100) : sizeDelta;
            
            if (relative)
                endValue += rectTransform.rect.size;

            return endValue;
        }

        private bool IsRectTransformStretched(RectTransform rectTransform, out bool isHorizontallyStretched, out bool isVerticallyStretched)
        {
            // Check if horizontal or vertical anchor is at the extremes (0 and 1) to determine horizontal or vertical stretching.
            isHorizontallyStretched = rectTransform.anchorMin.x == 0f && rectTransform.anchorMax.x == 1f;
            isVerticallyStretched = rectTransform.anchorMin.y == 0f && rectTransform.anchorMax.y == 1f;

            // If not horizontally or vertically stretched, check if anchor values are not in a specific set.
            if (!isHorizontallyStretched)
                isHorizontallyStretched = !(IsValueInSet(rectTransform.anchorMin.x) && IsValueInSet(rectTransform.anchorMax.x));
            if (!isVerticallyStretched)
                isVerticallyStretched = !(IsValueInSet(rectTransform.anchorMin.y) && IsValueInSet(rectTransform.anchorMax.y));

            return isHorizontallyStretched || isVerticallyStretched;

            // This static method checks if a value is in a specific set (0, 0.5, or 1).
            static bool IsValueInSet(float value) => value == 0f || value == 0.5f || value == 1f;
        }

        public override void ResetToInitialState()
        {
            if (targetRectTransform == null)
                return;

            targetRectTransform.sizeDelta = originalSize;
        }
    }
}
#endif