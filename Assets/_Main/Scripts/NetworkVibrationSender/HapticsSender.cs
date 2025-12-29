using System;
using System.Net.Sockets;
using System.Text;
using _Main.Scripts.Contracts.Events;
using UnityEngine;

namespace _Main.Scripts.NetworkVibrationSender
{
    public class HapticsSender : MonoBehaviour
    {
        [SerializeField] private string phoneIp = "192.168.0.37";

#if UNITY_ANDROID
        
        private const int Port = 5555;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            VibrationEvents.OnVibrate += Vibrate;
            VibrationEvents.OnCancel += Stop;
        }
        private void Vibrate(long duration, int intensity) 
            => Send(phoneIp, $"VIBRATE,{duration},{intensity}");
        private void Stop() => Send(phoneIp, "STOP");

        static void Send(string phoneIP, string msg)
        {
            using var udp = new UdpClient();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            udp.Send(data, data.Length, phoneIP, Port);
        }
#endif
    }
}