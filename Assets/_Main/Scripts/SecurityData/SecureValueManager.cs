using System;
using System.Collections.Generic;
using _Main.Scripts.CustomId;
using _Main.Scripts.MyComponents;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.SecurityData
{
    public class SecureValueManager : SingletonManagedBehaviour<SecureValueManager>, IUpdatable
    {
        #region Private Classes

        private interface ISecureValue
        {
            public event Action<ISecureValue> OnCheatDetected;
            public void RefreshProtection();
            public void SetValue(object value);
            
            public object GetValue();
        }

        [Serializable]
        private class SecureValue<T> : ISecureValue where T : struct
        {
            private byte[] _obfuscated;
            private int _key;
            private byte[] _checksum;

            private static readonly int SessionKey = UnityEngine.Random.Range(10000, 99999);
            
            public event Action<ISecureValue> OnCheatDetected;
            
            public SecureValue(T value, Action<ISecureValue> onCheatDetected)
            {
                SetValue(value);
                OnCheatDetected = onCheatDetected;
            }

            public T Value
            {
                get
                {
                    T val = Decrypt(_obfuscated);
                    if (!VerifyChecksum(val))
                        OnCheatDetected?.Invoke(this);
                    return val;
                }
                set => SetValue(value);
            }
            
            
            private void SetValue(T value)
            {
                _key = UnityEngine.Random.Range(1, int.MaxValue) ^ SessionKey;
                _obfuscated = Encrypt(value);
                _checksum = ComputeChecksum(value);
            }
            
            public void SetValue(object value)
            {
                if (value is T typed)
                {
                    Value = typed;
                }
                else
                {
                    throw new InvalidOperationException($"Type mismatch. Se esperaba {typeof(T)}");
                }
            }

            public object GetValue()
            {
                return Value;
            }

            public void RefreshProtection()
            {
                T realValue = Value; // valida checksum
                _key = UnityEngine.Random.Range(1, int.MaxValue) ^ SessionKey;
                _obfuscated = Encrypt(realValue);
                _checksum = ComputeChecksum(realValue);
            }
            
            private byte[] Encrypt(T value)
            {
                byte[] bytes = StructToBytes(value);
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] ^= (byte)(_key & 0xFF);
                return bytes;
            }

            private T Decrypt(byte[] data)
            {
                byte[] bytes = (byte[])data.Clone();
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] ^= (byte)(_key & 0xFF);
                return BytesToStruct<T>(bytes);
            }

            private byte[] ComputeChecksum(T value)
            {
                byte[] bytes = StructToBytes(value);
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = (byte)~bytes[i];
                return bytes;
            }

            private bool VerifyChecksum(T value)
            {
                byte[] check = ComputeChecksum(value);
                if (check.Length != _checksum.Length) return false;
                for (int i = 0; i < check.Length; i++)
                    if (check[i] != _checksum[i]) return false;
                return true;
            }

            #region Helpers de conversión
            private static byte[] StructToBytes(T value)
            {
                int size = System.Runtime.InteropServices.Marshal.SizeOf<T>();
                byte[] arr = new byte[size];
                IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                System.Runtime.InteropServices.Marshal.StructureToPtr(value, ptr, true);
                System.Runtime.InteropServices.Marshal.Copy(ptr, arr, 0, size);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                return arr;
            }

            private static U BytesToStruct<U>(byte[] bytes) where U : struct
            {
                int size = System.Runtime.InteropServices.Marshal.SizeOf<U>();
                IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, size);
                U obj = System.Runtime.InteropServices.Marshal.PtrToStructure<U>(ptr);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                return obj;
            }
            #endregion
        }
        #endregion
        
        private const int DefaultCapacity = 10;
        
        private readonly Dictionary<ushort, ISecureValue> _secureValuesById = new(DefaultCapacity);
        private readonly Dictionary<ISecureValue, ushort> _idBySecureValues = new(DefaultCapacity);
        
        private readonly List<ISecureValue> _secureValues = new(DefaultCapacity);
        private readonly List<ISecureValue> _toAdd = new(DefaultCapacity);
        private readonly List<ISecureValue> _toRemove = new(DefaultCapacity);
        
        private CustomIdGenerator _idGenerator = new();
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; private set; } = TickGroup.EverySecond;
        
        public static event Action<ushort> OnCheatDetected;

        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                foreach (var item in _toAdd)
                {
                    _secureValues.Add(item);
                }
                
                _toAdd.Clear();
            }

            if (_toRemove.Count > 0)
            {
                foreach (var item in _toAdd)
                {
                    item.OnCheatDetected -= OnCheatDetectedHandler;
                    _secureValues.Remove(item);
                }
                
                _toRemove.Clear();
            }
        }

        public void ExecuteUpdate(float deltaTime)
        {
            ApplyPending();
            
            foreach (var t in _secureValues)
            {
                t.RefreshProtection();
            }
        }

        #region Public API

        public static GeneratedId RegisterValue<T>(T startValue = default) where T : struct 
            => Instance.Internal_RegisterValue<T>(startValue);
        
        public static void RemoveValue(GeneratedId valueId)
            => Instance.Internal_RemoveValue(valueId);
        
        public static void ModifyValue<T>(GeneratedId valueId, T value) where T : struct 
            => Instance.Internal_ModifyValue<T>(valueId,value);
        
        public static bool GetDoesContainValue<T>(GeneratedId valueId, out T value) where T : struct 
            => Instance.Internal_GetDoesContainValue<T>(valueId, out value);

        #endregion
        
        #region Internal API

        private GeneratedId Internal_RegisterValue<T>(T startValue) where T : struct
        {
            var generatedId = _idGenerator.Generate();
            var valueToAdd = new SecureValue<T>(startValue,OnCheatDetectedHandler);
            
            
            _secureValuesById.Add(generatedId.Id, valueToAdd);
            _idBySecureValues.Add(valueToAdd, generatedId.Id);
            
            return generatedId;
        }

        private void Internal_RemoveValue(GeneratedId valueId)
        {
            if(valueId == null) return;
            
            if (_secureValuesById.ContainsKey(valueId.Id))
            {
                _secureValuesById.Remove(valueId.Id);
            }
        }

        private void Internal_ModifyValue<T>(GeneratedId valueId, T value) where T : struct
        {
            if(valueId == null) return;
            
            if (_secureValuesById.TryGetValue(valueId.Id, out var storedValue))
            {
                storedValue.SetValue(value);
            }
        }

        private bool Internal_GetDoesContainValue<T>(GeneratedId valueId, out T value) where T : struct
        {
            if (valueId == null)
            {
                value = default;
                return false;
            }

            if (_secureValuesById.TryGetValue(valueId.Id, out var secureValue))
            {
                value = (T)secureValue.GetValue();
                return true;
            }
            
            value = default!;
            return false;
        }

        #endregion
        
        #region Handlers
        
        private void OnCheatDetectedHandler(ISecureValue value)
        {
            
            var id = _idBySecureValues[value];
            
            OnCheatDetected?.Invoke(id);
        }
        
        #endregion
    }
}