using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class TestClass 
{

    public int INT = 4;
    public int INT2 = 5;
    public string STRING = "string";

    public TestClass testc;


    public byte[] output;
    //public TestClass testOutput;

    public void Tt()
    {
        //testc = new TestClass();
        //testc.INT = 44;
    }


    public static byte[] Serialize(object obj)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, obj);

            return memoryStream.ToArray();
        }
    }


    public static TestClass Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(data, 0, data.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (TestClass)binaryF.Deserialize(memoryStream);
        }
    }

}