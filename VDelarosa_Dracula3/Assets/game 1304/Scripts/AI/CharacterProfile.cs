using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZodiacSign { Capricorn, Aquarius, Pisces, Aries, Taurus,Gemini,Cancer,Leo,Virgo,Libra,Scorpio, Sagitarius};
public enum BloodType { APositive, ANegative, BPositive, BNegative, ABPositive, ABNegative, OPositive, ONegative};
public class CharacterProfile : MonoBehaviour
{
    public string characterName;
    public ZodiacSign zodiac;
    public BloodType bloodType;
    public int age;
    public int weight;    
}
