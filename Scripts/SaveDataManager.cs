//from https://answers.unity.com/questions/1300019/how-do-you-save-write-and-load-from-a-file.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameData
{
    public Vector3[,] mFlowField;

    public GameData(Vector3[,] _flowField)
    {
        mFlowField = _flowField;
    }
}
public class SaveDataManager : MonoBehaviour
{

    public void SaveFile(Vector3[,] _flowField, string _fileName)
    {
        string destination = Application.persistentDataPath + "/" + _fileName + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(_flowField);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public Vector3[,] LoadFile(string _fileName)
    {
        string destination = Application.persistentDataPath + "/" + _fileName + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();
        return data.mFlowField;

        
    }

}
