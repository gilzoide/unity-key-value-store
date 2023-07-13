#if UNITY_2018_1_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class StructBinarySerializer : IBinarySerializer
    {
        unsafe public byte[] SerializeObject<T>(T obj)
        {
            Debug.AssertFormat(UnsafeUtility.IsUnmanaged(typeof(T)), "Expected an unmanaged type, got {0}", typeof(T));
            int sizeOfT = UnsafeUtility.SizeOf(typeof(T));
            var buffer = new byte[sizeOfT];
            fixed (void* bufferPtr = buffer)
            {
                UnsafeUtility.MemCpy(bufferPtr, UnsafeUtility.AddressOf(ref UnsafeUtility.As<T, int>(ref obj)), sizeOfT);
            }
            return buffer;
        }

        unsafe public bool TryDeserializeObject<T>(byte[] bytes, out T value)
        {
            Debug.AssertFormat(UnsafeUtility.IsUnmanaged(typeof(T)), "Expected an unmanaged type, got {0}", typeof(T));
            value = default;
            int sizeOfT = UnsafeUtility.SizeOf(typeof(T));
            if (bytes.Length < sizeOfT)
            {
                return false;
            }

            fixed (void* bufferPtr = bytes)
            {
                UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref UnsafeUtility.As<T, int>(ref value)), bufferPtr, sizeOfT);
            }
            return true;
        }
    }
}
#endif