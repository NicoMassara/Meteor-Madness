﻿using System;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts
{
    public class InputTester : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                GameManager.Instance.EventManager.Publish(new MeteorDeflected());
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                GameManager.Instance.EventManager.Publish(new MeteorCollision());
            }
        }
    }
}