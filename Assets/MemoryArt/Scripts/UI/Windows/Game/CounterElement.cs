using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryArt.UI.Windows
{
    public class CounterElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _filledOutline;
        [SerializeField] private Image _tickImage;

        private Tween _animation;
        private Sequence _transformToDoneAnimation;

        //add button click listener, init transform to btn animation
        private void Start()
        {
            _transformToDoneAnimation = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    _tickImage.enabled = true;
                    _text.enabled = false;
                })
                .Append(transform.DOScale(1.2f, .3f))
                .Append(transform.DOScale(1f, .3f))
                .Pause()
                .SetAutoKill(false);
        }

        public void SetDefaults()
        {
            transform.localScale = Vector3.one;
            _tickImage.enabled = false;
            _text.enabled = true;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public Sequence TimerTween(int seconds, string secondsText)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_filledOutline.DOFillAmount(0, seconds).SetEase(Ease.Linear));
            for (int i = 0; i <= seconds; i++)
            {
                var currSeconds = seconds - i;
                sequence.InsertCallback(i, () => SetText($"{currSeconds}{secondsText}"));
            }

            return sequence;
        }

        public void SetAmount(float fillAmount, float duration = 0, Ease ease = Ease.Linear)
        {
            _animation?.Kill();
            if (duration == 0)
            {
                _filledOutline.fillAmount = fillAmount;
            }
            else
            {
                _animation = _filledOutline.DOFillAmount(fillAmount, duration).SetEase(ease);
                _animation.Play();
            }
        }

        public void SetColor(Color color)
        {
            _filledOutline.color = color;
        }

        public void TransformToDone(Color color)
        {
            if (_animation != null && _animation.IsPlaying())
            {
                _animation.OnComplete(() =>
                {
                    SetColor(color);
                    _transformToDoneAnimation.Restart();
                });
            }
            else
            {
                SetColor(color);
                _transformToDoneAnimation.Restart();
            }
        }
    }
}