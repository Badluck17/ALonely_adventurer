using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zycalipse.Systems
{
    public class AutoScrollElementSystem : MonoBehaviour
    {
        [SerializeField]
        private AudioSource counterSound;
        [SerializeField]
        private int startPosition, endPosition;
        private int[] eachItemPos = new int[6];
        [Header("Put negative/positive value to scroll speed to change the direction")]
        [SerializeField]
        private float scrollSpeed;
        public float timer;
        public bool Starting { get; set; }
        public bool Done { get; set; }
        private int itemSelectedNumber = 0;

        public RectTransform rectTransform;
        public RectTransform parentRectTransform;
        private Vector2 startPos;

        private void Start()
        {
            counterSound.Stop();
            startPos = rectTransform.anchoredPosition;

            Done = false;

            for (int i = 0; i < eachItemPos.Length; i++)
            {
                if (i == 0)
                    eachItemPos[i] = endPosition;
                else
                    eachItemPos[i] = eachItemPos[i - 1] - 350;
            }
        }

        private void OnDisable()
        {
            Starting = false;
        }
        private void OnEnable()
        {
            Starting = true;
        }

        void Update()
        {
            if (timer > 0)
            {
                if(Starting)
                    timer -= Time.deltaTime;

                if (!Starting)
                    Starting = true;
            }
            else
            {
                if (!Done)
                {
                    // stop loop sound
                    if (counterSound.isPlaying)
                        counterSound.Stop();

                    // find nearest item for last scroll
                    float lastValue = 9999f;
                    for (int i = 0; i < eachItemPos.Length; i++)
                    {
                        // because the desired pos is in both negative and positive
                        // check the current position depending on type
                        // if negative skip until found negative vice versa
                        // then find the nearest

                        if (rectTransform.anchoredPosition.y < 0)
                        {
                            if (eachItemPos[i] > 0)
                                continue;

                            var value = rectTransform.anchoredPosition.y - eachItemPos[i];

                            if (lastValue == 9999f)
                                itemSelectedNumber = i;
                            else
                                itemSelectedNumber = value > lastValue ? i : itemSelectedNumber;

                            lastValue = value;
                        }
                        else
                        {
                            if (eachItemPos[i] < 0)
                                continue;

                            var value = rectTransform.anchoredPosition.y - eachItemPos[i];

                            if(lastValue == 9999f)
                                itemSelectedNumber = i;
                            else
                                itemSelectedNumber = value < lastValue ? i : itemSelectedNumber;

                            lastValue = value;
                        }
                    }

                    if (itemSelectedNumber == eachItemPos.Length)
                        itemSelectedNumber = 0;

                    Managers.SoundManager.PlayOneShotSound(Managers.DataManager.Instance.WheelSkillStop, transform.position);
                    var destination = new Vector2(rectTransform.anchoredPosition.x, eachItemPos[itemSelectedNumber]);
                    rectTransform.anchoredPosition = destination;
                    Starting = false;
                    Done = true;
                }

                return;
            }

            if (Starting)
            {
                // start sound loop
                if(!counterSound.isPlaying)
                    counterSound.Play();

                if (rectTransform.anchoredPosition.y < startPosition)
                {
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, endPosition);
                }
                else if (rectTransform.anchoredPosition.y > endPosition)
                {
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPosition);
                }
                else
                {
                    var rand = Managers.GameManager.Instance.PrecentageRandomizer();
                    rand += (int)Time.realtimeSinceStartup;
                    rand %= 4;
                    rand++;

                    float move = rand * scrollSpeed * Time.deltaTime;
                    rectTransform.anchoredPosition += new Vector2(0, move);
                }
            }
        }

        public int SelectedItemsIndex()
        {
            return itemSelectedNumber;
        }

        public void ResetSpin()
        {
            rectTransform.anchoredPosition = startPos;
            Done = false;
            timer = 4f;
            itemSelectedNumber = 0;
        }
    }
}
