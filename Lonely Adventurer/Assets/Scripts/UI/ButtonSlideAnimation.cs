using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.UI
{
    public class ButtonSlideAnimation : MonoBehaviour
    {
        [SerializeField]
        private Sprite selectedSprite, unselectedSprite;
        [SerializeField]
        private Image buttonImage;
        [SerializeField]
        private RectTransform iconTransform, endPosition, startPosition, thisRectTransform;
        private float speed = 500f;
        [SerializeField]
        private GameObject menuText;
        public bool Selected { get; set; }

        private void Start()
        {
            thisRectTransform.anchoredPosition = startPosition.anchoredPosition;
            Selected = false;
            menuText.SetActive(Selected);
        }

        public async void DoSlide()
        {
            if (Selected)
                await SlideToEndPosition();
            else
                SlideToStartPosition();
        }

        private async Task SlideToEndPosition()
        {
            if (thisRectTransform == null || iconTransform == null)
                return;

            iconTransform.anchoredPosition = new Vector2(iconTransform.anchoredPosition.x, 40);
            iconTransform.localScale = new Vector3(1f, 1f, 1f);
            while (thisRectTransform != null && thisRectTransform.anchoredPosition.y < endPosition.anchoredPosition.y)
            {
                thisRectTransform.anchoredPosition =
                               new Vector2(thisRectTransform.anchoredPosition.x, thisRectTransform.anchoredPosition.y + speed * Time.deltaTime);

                if(thisRectTransform.anchoredPosition.y > endPosition.anchoredPosition.y)
                    thisRectTransform.anchoredPosition =
                               new Vector2(thisRectTransform.anchoredPosition.x, endPosition.anchoredPosition.y);

                await Task.Yield();
            }
            menuText.SetActive(Selected);
            buttonImage.sprite = selectedSprite;
        }

        private async void SlideToStartPosition()
        {
            if (thisRectTransform == null || iconTransform == null)
                return;

            buttonImage.sprite = unselectedSprite;
            menuText.SetActive(Selected);
            while (thisRectTransform != null && thisRectTransform.anchoredPosition.y > startPosition.anchoredPosition.y)
            {
                thisRectTransform.anchoredPosition =
                               new Vector2(thisRectTransform.anchoredPosition.x, thisRectTransform.anchoredPosition.y - speed * Time.deltaTime);

                if (thisRectTransform.anchoredPosition.y < startPosition.anchoredPosition.y)
                    thisRectTransform.anchoredPosition =
                               new Vector2(thisRectTransform.anchoredPosition.x, startPosition.anchoredPosition.y);

                await Task.Yield();
            }
            iconTransform.anchoredPosition = new Vector2(iconTransform.anchoredPosition.x, 0);
            iconTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
    }
}