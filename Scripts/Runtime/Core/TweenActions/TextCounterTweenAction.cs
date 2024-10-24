#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    // Created by Pablo Huaxteco
    [Serializable]
    public sealed class TextCounterTweenAction : TweenActionBase
    {
        public override Type TargetComponentType => typeof(Text);
        public override string DisplayName => "Counter";

        [SerializeField]
        private int counter;
        public int Counter
        {
            get => counter;
            set => counter = value;
        }

        [SerializeField]
        private bool addThousandsSeparator = true;
        public bool AddThousandsSeparator
        {
            get => addThousandsSeparator;
            set => addThousandsSeparator = value;
        }

        private Text targetText;
        private string originalText;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetText == null || targetText.gameObject != target)
            {
                targetText = target.GetComponent<Text>();
                if (targetText == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component.");
                    return null;
                }
            }

            originalText = targetText.text;

            int startCounter = 0;
            if (int.TryParse(targetText.text, out int result))
                startCounter = result;

            TweenerCore<int, int, NoOptions> tween = targetText.DOCounter(startCounter, counter, duration, addThousandsSeparator);

            return tween;
        }

        public override void ResetToInitialState()
        {
            if (targetText == null)
                return;

            if (string.IsNullOrEmpty(originalText))
                return;

            targetText.text = originalText;
        }
    }
}
#endif