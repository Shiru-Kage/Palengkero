using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string id;
    public string itemName;
    public int price;
    public int nutrition;
    public int satisfaction;
    public int stockLimit;
}

[System.Serializable]
public class ItemDatabase {
    public List<ItemData> items;
}
