using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SerializationHelper
{
    public static byte[] Serialize<T>(T data)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, data);
                return ms.ToArray();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"序列化失败: {e.Message}");
            return null;
        }
    }

    public static T Deserialize<T>(byte[] data)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return (T)formatter.Deserialize(ms);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"反序列化失败: {e.Message}");
            return default(T);
        }
    }
}
