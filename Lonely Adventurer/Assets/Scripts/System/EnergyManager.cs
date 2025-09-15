using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zycalipse.Managers;

namespace Zycalipse.Systems
{
    public class EnergyManager : MonoBehaviour
    {
        private static EnergyManager instance;
        public static EnergyManager Instance
        {
            get
            {
                return instance;
            }
        }

        private Text energyText, timerText;
        private int maxEnergy = 25;
        private int currentEnergy;
        private int restoreDuration = 1;
        private DateTime nextEnergyTime, lastEnergyTime;
        private bool isRestoring = false;

        private string currentEnergyName = "CurrentEnergy";
        private string lastEnergyTimeName = "LastEnergyTime";
        private string nextEnergyTimeName = "NextEnergyTime";

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
        }

        async void Start()
        {
            while (Menus.MainMenu.Instance == null)
            {
                await System.Threading.Tasks.Task.Yield();
            }

            energyText = Menus.MainMenu.Instance.energyText;
            timerText = Menus.MainMenu.Instance.energyTimerText;

            if (!PlayerPrefs.HasKey(lastEnergyTimeName))
            {
                PlayerPrefs.SetInt(currentEnergyName, 25);
            }
            LoadData();
            StartCoroutine("RestoreEnergy");
        }

        public bool UseEnergy()
        {
            if (currentEnergy >= 5)
            {
                currentEnergy -= 5;
                UpdateEnergyText();

                if (!isRestoring)
                {
                    if (currentEnergy + 1 == maxEnergy)
                    {
                        nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
                    }
                    StartCoroutine("RestoreEnergy");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator RestoreEnergy()
        {
            UpdateEnergyTimer();
            isRestoring = true;

            while (currentEnergy < maxEnergy)
            {
                DateTime currentTime = DateTime.Now;
                DateTime nextTime = nextEnergyTime;
                bool isAddingEnergy = false;

                while (currentTime > nextTime)
                {
                    if(currentEnergy < maxEnergy)
                    {
                        isAddingEnergy = true;
                        currentEnergy++;
                        UpdateEnergyText();
                        DateTime timeToAdd = lastEnergyTime > nextTime ? lastEnergyTime : nextTime;
                        nextTime = AddDuration(timeToAdd, restoreDuration);
                    }
                    else
                    {
                        break;
                    }
                }

                if (isAddingEnergy)
                {
                    lastEnergyTime = DateTime.Now;
                    nextEnergyTime = nextTime;
                }

                UpdateEnergyTimer();
                UpdateEnergyText();
                SaveData();
                yield return null;
            }

            isRestoring = false;
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt(currentEnergyName, currentEnergy);
            PlayerPrefs.SetString(lastEnergyTimeName, lastEnergyTime.ToString());
            PlayerPrefs.SetString(nextEnergyTimeName, nextEnergyTime.ToString());
        }

        private void LoadData()
        {
            currentEnergy = PlayerPrefs.GetInt(currentEnergyName);
            lastEnergyTime = StringToDateTime(PlayerPrefs.GetString(lastEnergyTimeName));
            nextEnergyTime = StringToDateTime(PlayerPrefs.GetString(nextEnergyTimeName));
        }

        private DateTime StringToDateTime(string date)
        {
            if (String.IsNullOrEmpty(date))
                return DateTime.Now;
            else
                return DateTime.Parse(date);
        }

        private void UpdateEnergyText()
        {
            energyText.text = $"{currentEnergy}/{maxEnergy}";
        }

        private void UpdateEnergyTimer()
        {
            if (currentEnergy < maxEnergy)
            {
                TimeSpan time = nextEnergyTime - DateTime.Now;
                string timeVal = $"{time.Minutes}:{time.Seconds}";
                timerText.text = timeVal;
            }
            else
            {
                timerText.text = "FULL";
            }
        }

        private DateTime AddDuration(DateTime time, double duration)
        {
            return time.AddMinutes(duration);
        }
    }
}
