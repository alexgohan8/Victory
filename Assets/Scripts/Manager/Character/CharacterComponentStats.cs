using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentStats : MonoBehaviour
{
    //trainedStats and freedom are used in training component
    private Character character;

    [SerializeField] private Dictionary<Stat, int> baseStats = new();       //from character data
    [SerializeField] private Dictionary<Stat, int> trainedStats = new();    //gained from training
    [SerializeField] private Dictionary<Stat, int> trueStats = new();       //calculated after scaling baseStats
    [SerializeField] private Dictionary<Stat, int> battleStats = new();     //used during battle

    [Range(0f, 1f)] private float minStatRatioHp = 0.4f; // 40% of base at level 1
    [Range(0f, 1f)] private float minStatRatioSp = 0.4f; // 40% of base at level 1
    [Range(0f, 1f)] private float minStatRatioOther = 0.1f; // 10% of base at level 1

    private const int baseLevel = 50;
    private const int midLevel = 99;
    private const int maxLevel = 200;
    private float midPhaseGrowth = 0.6f;  // growth strength between 50→99 (1.0→1.6× curve)
    private float highPhaseGrowth = 0.4f; // growth strength between 99→200 (continues slower)

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            int baseStatValue = GetBaseStatValueFromData(stat, characterData);
            baseStats[stat] = baseStatValue;
            trainedStats[stat] = 0;
            trueStats[stat] = 0;
            battleStats[stat] = 0;
        }

        UpdateStats();
    }

    private int GetBaseStatValueFromData(Stat stat, CharacterData characterData)
    {
        return stat switch
        {
            Stat.Gp => characterData.Gp,
            Stat.Tp => characterData.Tp,
            Stat.Kick => characterData.Kick,
            Stat.Control => characterData.Control,
            Stat.Technique => characterData.Technique,
            Stat.Pressure => characterData.Pressure,
            Stat.Physical => characterData.Physical,
            Stat.Agility => characterData.Agility,
            Stat.Intelligence => characterData.Intelligence,
            _ => 0
        };
    }

    public int GetTrainedStat(Stat stat) => trainedStats[stat];
    public int GetTrueStat(Stat stat) => trueStats[stat];
    public int GetBattleStat(Stat stat) => battleStats[stat];

    public void ResetBattleStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            battleStats[stat] = trueStats[stat];
        }
    }

    public void ModifyBattleStat(Stat stat, int amount)
    {
        if (stat == Stat.Gp && amount < 0) 
            amount = GetReducedHpAmount(amount);
        //battleStats[stat] = Mathf.Clamp(battleStats[stat] + amount, 0, trueStats[stat]);
        battleStats[stat] = Mathf.Clamp(battleStats[stat] + amount, 0, 999);
        if (stat == Stat.Gp) this.character.UpdateFatigue();
    }

    public void ModifyTrainedStat(Stat stat, int amount)
    {
        trainedStats[stat] = Mathf.Clamp(trainedStats[stat] + amount, 0, this.character.MaxTrainingPerStat);
    }

    public void ResetTrainedStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            trainedStats[stat] = 0;
    }

    public void UpdateStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
        {
            trueStats[stat] = ScaleStat(baseStats[stat], stat) + trainedStats[stat];
            battleStats[stat] = trueStats[stat];   
        }
    }

    private int ScaleStat(int baseStat, Stat stat)
    {
        int level = Mathf.Clamp(this.character.Level, 1, maxLevel);

        float minRatio = (stat == Stat.Gp) ? minStatRatioHp :
                         (stat == Stat.Tp) ? minStatRatioSp :
                         minStatRatioOther;

        float ratio;

        if (level <= baseLevel)
        {
            // Phase 1: Level 1→50
            float t = Mathf.InverseLerp(1, baseLevel, level);
            ratio = Mathf.Lerp(minRatio, 1f, t * t);
        }
        else if (level <= midLevel)
        {
            // Phase 2: Level 51→99
            float t = Mathf.InverseLerp(baseLevel, midLevel, level);
            // 1.0 → (1 + midPhaseGrowth)
            ratio = Mathf.Lerp(1f, 1f + midPhaseGrowth, t * t);
        }
        else
        {
            // Phase 3: Level 100→200
            float t = Mathf.InverseLerp(midLevel, maxLevel, level);
            // continues from previous value  (1 + midPhaseGrowth) → add more smoothly
            float start = 1f + midPhaseGrowth;
            float end = start + highPhaseGrowth; // (1.6 → 2.0)
            ratio = Mathf.Lerp(start, end, t * t);
        }

        float value = baseStat * ratio;
        return Mathf.RoundToInt(value);
    }

    private int GetReducedHpAmount(int amount)
    {
        //amount is negative
        const float LvReductionPerLevel = 0.01f;
        const float MaxLvReduction = 0.7f;
        const float StaminaDivisor = 130f;
        const float MaxStaminaReduction = 0.3f;
        const float MinDamageTaken = -1f;

        float stamina = battleStats[Stat.Physical];

        // Calculate reduction factors
        float lvFactor = 1f - Mathf.Min(this.character.Level * LvReductionPerLevel, MaxLvReduction);
        float staminaFactor = 1f - Mathf.Min(stamina / StaminaDivisor, MaxStaminaReduction);

        // Combine both (multiplicative, so boosts "stack" in reducing damage)
        float totalFactor = lvFactor * staminaFactor;

        // Ensure damage never goes below a minimum (e.g., at least 1)
        float damageTaken = Mathf.Min(MinDamageTaken, amount * totalFactor);

        return (int)damageTaken;
    }

    /*
    Input
    Gp	Tp	Kick	Control	Technique	Pressure	Physical	Agility
    100	100	86	94	88	98	102	104

    Output
    Level	HP / SP Ratio	Kick / Etc Ratio	Gp (HP)	Kick	Control	Technique	Pressure	Physical	Agility
    1	0.40	0.10	40	9	9	9	10	10	10
    25	0.75	0.55	75	47	52	48	54	56	57
    50	1.00	1.00	100	86	94	88	98	102	104
    75	1.36	1.36	136	117	128	120	133	139	141
    99	1.60	1.60	160	138	150	141	157	163	166
    150	1.86	1.86	186	160	174	164	183	190	193
    200	2.00	2.00	200	172	188	176	196	204	208

    */

}
