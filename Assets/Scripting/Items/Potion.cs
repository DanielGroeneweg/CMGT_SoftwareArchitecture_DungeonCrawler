using UnityEngine;
public enum PotionTypes { health, stamina }
[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/Potion")]
public class Potion : Item
{
    public PotionTypes type;
    public int regenAmount;
}