using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public  ItemDatabaseObject database;
    public Inventory Container;

    public void AddItem(Item _item, int _amount)
    {
        if (_item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount);
            return;
        }
        for (int i = 0; i < Container.Item.Length; i++)
        {
            if (Container.Item[i].ID == _item.id)
            {
                Container.Item[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);
    }
    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Item.Length; i++)
        {
            if(Container.Item[i].ID <= -1)
            {
                Container.Item[i].UpdateSlot(_item.id, _item, _amount);
                return Container.Item[i];
            }
        }

        // set up functionaly for full Inventory
        return null;
    }
    public void MoveItem(InventorySlot item1,InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID,item2.Item,item2.amount);
        item2.UpdateSlot(item1.ID, item1.Item,item1.amount);
        item1.UpdateSlot(temp.ID, temp.Item,temp.amount);   
    }
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Item.Length; i++)
        {
            if(Container.Item[i].Item == _item)
            {
                Container.Item[i].UpdateSlot(-1, null, 0);
            }
        }
    }
    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this,true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath),FileMode.Create,FileAccess.Write);
        formatter.Serialize(stream, Container); 
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath,savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Item.Length; i++)
            {
                Container.Item[i].UpdateSlot(newContainer.Item[i].ID, newContainer.Item[i].Item, newContainer.Item[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Item = new InventorySlot[24];
}
[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public Item Item;
    public int amount;
    public InventorySlot()
    {
        ID = -1;
        Item = null;
        amount = 0;
    }
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        Item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        Item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
