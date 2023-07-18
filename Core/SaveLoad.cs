using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using DesktopAI.Models;

namespace DesktopAI.Core;

public static class SaveLoad
{
    public static void Save(Key obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream("key.dat", FileMode.Create);
#pragma warning disable SYSLIB0011
        formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011

        stream.Close();
    }
    
    public static Key Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {            
            FileStream stream = new FileStream("key.dat", FileMode.Open);
#pragma warning disable SYSLIB0011
            Key obj = (Key)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            stream.Close();
            return obj;
        }
        catch
        {
            return new Key("");
        }

        

    }
}