#if UNITY_2018_1_OR_NEWER
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Gilzoide.KeyValueStore.Utils;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class UnityMathTextSerializer : ITextSerializer<Color>,
        ITextSerializer<Quaternion>,
        ITextSerializer<Matrix4x4>,
        ITextSerializer<Plane>,
        ITextSerializer<Ray>, ITextSerializer<Ray2D>,
        ITextSerializer<RangeInt>,
        ITextSerializer<Rect>, ITextSerializer<RectInt>,
        ITextSerializer<Vector2>, ITextSerializer<Vector2Int>,
        ITextSerializer<Vector3>, ITextSerializer<Vector3Int>,
        ITextSerializer<Vector4>
    {
        public const char NumberSeparator = ',';

        public void RegisterInSerializerMap(ObjectSerializerMap serializerMap)
        {
            serializerMap.SetObjectSerializer<Color>(this);
            serializerMap.SetObjectSerializer<Quaternion>(this);
            serializerMap.SetObjectSerializer<Matrix4x4>(this);
            serializerMap.SetObjectSerializer<Plane>(this);
            serializerMap.SetObjectSerializer<Ray>(this);
            serializerMap.SetObjectSerializer<Ray2D>(this);
            serializerMap.SetObjectSerializer<RangeInt>(this);
            serializerMap.SetObjectSerializer<Rect>(this);
            serializerMap.SetObjectSerializer<RectInt>(this);
            serializerMap.SetObjectSerializer<Vector2>(this);
            serializerMap.SetObjectSerializer<Vector2Int>(this);
            serializerMap.SetObjectSerializer<Vector3>(this);
            serializerMap.SetObjectSerializer<Vector3Int>(this);
            serializerMap.SetObjectSerializer<Vector4>(this);
        }

        public bool TryDeserializeObject(string text, out Color value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Color value)
        {
            return SerializeFloats(value);
        }

        public bool TryDeserializeObject(string text, out Vector2 value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Vector2 value)
        {
            return SerializeFloats(value);
        }
        public bool TryDeserializeObject(string text, out Vector2Int value)
        {
            return TryDeserializeInts(text, out value);
        }
        public string SerializeObject(Vector2Int value)
        {
            return SerializeInts(value);
        }

        public bool TryDeserializeObject(string text, out Vector3 value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Vector3 value)
        {
            return SerializeFloats(value);
        }
        public bool TryDeserializeObject(string text, out Vector3Int value)
        {
            return TryDeserializeInts(text, out value);
        }
        public string SerializeObject(Vector3Int value)
        {
            return SerializeInts(value);
        }

        public bool TryDeserializeObject(string text, out Vector4 value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Vector4 v)
        {
            return SerializeFloats(v);
        }
        public bool TryDeserializeObject(string text, out Quaternion value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Quaternion value)
        {
            return SerializeFloats(value);
        }

        public bool TryDeserializeObject(string text, out Matrix4x4 value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Matrix4x4 value)
        {
            return SerializeFloats(value);
        }

        public bool TryDeserializeObject(string text, out RangeInt value)
        {
            return TryDeserializeInts(text, out value);
        }
        public string SerializeObject(RangeInt value)
        {
            return SerializeInts(value);
        }

        public bool TryDeserializeObject(string text, out Rect value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Rect value)
        {
            return SerializeFloats(value);
        }
        public bool TryDeserializeObject(string text, out RectInt value)
        {
            return TryDeserializeInts(text, out value);
        }
        public string SerializeObject(RectInt value)
        {
            return SerializeInts(value);
        }

        public bool TryDeserializeObject(string text, out Plane value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Plane value)
        {
            return SerializeFloats(value);
        }

        public bool TryDeserializeObject(string text, out Ray value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Ray value)
        {
            return SerializeFloats(value);
        }
        public bool TryDeserializeObject(string text, out Ray2D value)
        {
            return TryDeserializeFloats(text, out value);
        }
        public string SerializeObject(Ray2D value)
        {
            return SerializeFloats(value);
        }

        #region Internal unsafe serialization

        unsafe private static string SerializeInts<T>(T value) where T : struct
        {
            int intCount = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<int>();
            void* ptr = UnsafeUtility.AddressOf(ref value);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(UnsafeUtility.ReadArrayElement<int>(ptr, 0).ToString(CultureInfo.InvariantCulture));
            for (int i = 1; i < intCount; i++)
            {
                stringBuilder.Append(NumberSeparator);
                stringBuilder.Append(UnsafeUtility.ReadArrayElement<int>(ptr, i).ToString(CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }

        unsafe private static bool TryDeserializeInts<T>(string text, out T value) where T : struct
        {
            value = default;
            void* ptr = UnsafeUtility.AddressOf(ref value);
            int intCount = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<int>();

            using (IEnumerator<int> enumerator = text.EnumerateInts(NumberSeparator).GetEnumerator())
            for (int i = 0; i < intCount; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return false;
                }
                UnsafeUtility.WriteArrayElement(ptr, i, enumerator.Current);
            }
            return true;
        }

        unsafe private static string SerializeFloats<T>(T value) where T : struct
        {
            int floatCount = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<float>();
            void* ptr = UnsafeUtility.AddressOf(ref value);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(UnsafeUtility.ReadArrayElement<float>(ptr, 0).ToString(CultureInfo.InvariantCulture));
            for (int i = 1; i < floatCount; i++)
            {
                stringBuilder.Append(NumberSeparator);
                stringBuilder.Append(UnsafeUtility.ReadArrayElement<float>(ptr, i).ToString(CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }

        unsafe private static bool TryDeserializeFloats<T>(string text, out T value) where T : struct
        {
            value = default;
            void* ptr = UnsafeUtility.AddressOf(ref value);
            int floatCount = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<float>();

            using (IEnumerator<float> enumerator = text.EnumerateFloats(NumberSeparator).GetEnumerator())
            for (int i = 0; i < floatCount; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return false;
                }
                UnsafeUtility.WriteArrayElement(ptr, i, enumerator.Current);
            }
            return true;
        }

        #endregion
    }
}
#endif