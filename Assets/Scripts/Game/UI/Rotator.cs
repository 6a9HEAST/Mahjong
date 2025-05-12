using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        public  float rotationSpeed = 90f; // Скорость вращения (градусы в секунду)

        [SerializeField]
        public  Vector3 rotationAxis = Vector3.up; // Ось вращения

        void Update()
        {
           transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
}

