using UnityEngine;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string CharacterId;
    public string BodyTone;
    public PortraitSize PortraitSize;
    public CharacterSize CharacterSize;
    public Gender Gender;
    public Element Element;
    public Position Position;
    public int Gp; 
    public int Tp;
    public int Kick;
    public int Control;
    public int Technique;
    public int Pressure;
    public int Physical;
    public int Agility;
    public int Intelligence;
    public string[] MoveIds = new string[4];
    public int[] MoveLvs = new int[4];
}
