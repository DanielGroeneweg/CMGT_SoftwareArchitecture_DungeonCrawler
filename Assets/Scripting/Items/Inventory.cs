using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private int gold = 0;
    [SerializeField] private Weapon equippedSword;

    public void AddItem(Item item) { items.Add(item); }
    public void RemoveItem(Item item) { if (items.Contains(item)) items.Remove(item); }
    public void AddGold(int amount) { gold += Mathf.Abs(amount); }
    public void RemoveGold(int amount) { gold -= Mathf.Abs(amount); }
    public Weapon GetEquippedSword() { return equippedSword; }
}