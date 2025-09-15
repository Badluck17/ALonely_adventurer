using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zycalipse.Menus
{
    public class LevelSelectionMenuModel : MonoBehaviour
    {
        public Transform ModelGroup;

        [Header("Rotation Settings")]
        public float rotationSpeed = 5f; // smooth speed

        private Quaternion targetRotation;
        private int currentIndex = 0; // 0 = Arena, 1 = Shop, 2 = Equipment, 3 = Character
        private readonly int[] rotationAngles = { 0, 90, 180, 270 };

        [SerializeField]
        private GameObject[] cameras;

        void Start()
        {
            SetTargetRotation(rotationAngles[currentIndex]);
        }

        void Update()
        {
            ModelGroup.rotation = Quaternion.Lerp(ModelGroup.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        private void SetTargetRotation(int yRotation)
        {
            targetRotation = Quaternion.Euler(0, yRotation, 0);
        }

        public void NextMenu()
        {
            currentIndex = (currentIndex + 1) % rotationAngles.Length;
            SetTargetRotation(rotationAngles[currentIndex]);
        }

        public void PreviousMenu()
        {
            currentIndex = (currentIndex - 1 + rotationAngles.Length) % rotationAngles.Length;
            SetTargetRotation(rotationAngles[currentIndex]);
        }

        public void GoToMenu(int index)
        {
            if (index >= 0 && index < rotationAngles.Length)
            {
                currentIndex = index;
                SetTargetRotation(rotationAngles[currentIndex]);

                for (int i = 0; i < 3; i++)
                {
                    cameras[i].gameObject.SetActive(false);
                }

                cameras[index].gameObject.SetActive(true);
            }
        }
    }
}
