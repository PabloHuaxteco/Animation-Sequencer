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
    public sealed class CameraColorTweenAction : TweenActionBase
    {
        public override Type TargetComponentType => typeof(Camera);
        public override string DisplayName => "Color";

        [SerializeField]
        private Color color = Color.white;
        public Color Color
        {
            get => color;
            set => color = value;
        }

        private Camera targetCamera;
        private Color originalColor;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetCamera == null || targetCamera.gameObject != target)
            {
                targetCamera = target.GetComponent<Camera>();
                if (targetCamera == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component.");
                    return null;
                }
            }

            originalColor = targetCamera.backgroundColor;

            TweenerCore<Color, Color, ColorOptions> tween = targetCamera.DOColor(color, duration);

            return tween;
        }

        public override void ResetToInitialState()
        {
            if (targetCamera == null)
                return;

            targetCamera.backgroundColor = originalColor;
        }
    }
}
#endif