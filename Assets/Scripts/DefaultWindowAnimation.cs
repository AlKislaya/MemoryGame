using System;
using Dainty.UI.WindowBase;
using UnityEngine;

namespace Assets.Scripts
{
    public class DefaultWindowAnimation: AWindowAnimation
    {
        [SerializeField] private Canvas _canvas;
        
        public override void ShowImmediate()
        {
            _canvas.enabled = true;
        }

        public override void PlayShowAnimation(Action animationFinished = null)
        {
            throw new NotImplementedException();
        }

        public override void CloseImmediate()
        {
            _canvas.enabled = false;
        }

        public override void PlayCloseAnimation(Action animationFinished = null)
        {
            throw new NotImplementedException();
        }
    }
}
