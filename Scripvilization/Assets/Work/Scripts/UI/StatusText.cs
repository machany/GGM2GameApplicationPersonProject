using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Work.Scripts.UI
{
    public class StatusText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private string waitingText, executingText, repeatingText;
        [SerializeField] private string repeatAnimationText;
        [SerializeField] private float animationRate;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color repeatStatusColor;

        private string _statusText;

        private float _animationPlayedTime;
        private int animationFrame;

        private void Awake()
        {
            _animationPlayedTime = 0;
            animationFrame = 0;
            _statusText = waitingText;
        }

        private void Update()
        {
            if ((_animationPlayedTime += Time.deltaTime) > animationRate)
            {
                _animationPlayedTime = 0;
                animationFrame = (animationFrame + 1) % (repeatAnimationText.Length + 1);

                tmp.text = $"{_statusText}{repeatAnimationText.Substring(0, animationFrame)}";
            }
        }

        public void SetText(bool isExecute, bool isRepeat)
        {
            _statusText = isRepeat ? repeatingText : isExecute ? executingText : waitingText;
            tmp.color = isRepeat ? repeatStatusColor : defaultColor;
        }
    }
}
