using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public static class DamageCalculator
{
    // Multiplier constants
    private const float MAIN_MULTIPLIER = 0.5f;
    private const float SUB_MULTIPLIER = 0.1f;
    private const float MOVE_MULTIPLIER = 3.0f;
    private const float ELEMENT_MATCH_MULTIPLIER = 1.5f;
    public const float ELEMENT_EFFECTIVE_MULTIPLIER = 1.5f;

    private const float KEEPER_MULTIPLIER = 3f;

    private const float DISTANCE_MULTIPLIER = 10f;
    private const float DIRECT_BONUS = 50f;


    // Helper function for Melee/Ranged formulas
    private static float CalcFormula(Character character, Stat main, Stat sub0, Stat sub1)
    {
        return
            character.GetBattleStat(main) * MAIN_MULTIPLIER +
            character.GetBattleStat(sub0) * SUB_MULTIPLIER +
            character.GetBattleStat(sub1) * SUB_MULTIPLIER;
            //character.GetBattleStat(Stat.Courage);
    }

    // Helper function for Move formulas
    private static float CalcMove(Character character, Move move, Stat main)
    {
        if (move == null) return 0f;
        float baseDamage =
            move.Power * MOVE_MULTIPLIER +
            character.GetBattleStat(main);
            //character.GetBattleStat(Stat.Courage);
        if (character.Element == move.Element)
            baseDamage *= ELEMENT_MATCH_MULTIPLIER;
        return baseDamage;
    }

    // Special for Shoot (minus distance)
    private static float CalcDistanceReduction(Character character)
    {
        return GoalManager.Instance.GetDistanceToOpponentGoal(character) * DISTANCE_MULTIPLIER;
    }

    public static Dictionary<(Category, DuelCommand), Func<Character, Move, float>> damageFormulas =
        new Dictionary<(Category, DuelCommand), Func<Character, Move, float>>()
    {
        // Dribble
        {(Category.Dribble, DuelCommand.Melee),     (character, move) => CalcFormula(character, Stat.Technique, Stat.Intelligence, Stat.Pressure)},
        {(Category.Dribble, DuelCommand.Ranged),    (character, move) => CalcFormula(character, Stat.Technique, Stat.Control, Stat.Kick)},
        {(Category.Dribble, DuelCommand.Move),      (character, move) => CalcMove(character, move, Stat.Technique)},

        // Block
        {(Category.Block, DuelCommand.Melee),   (character, move) => CalcFormula(character, Stat.Pressure, Stat.Physical, Stat.Physical)},
        {(Category.Block, DuelCommand.Ranged),  (character, move) => CalcFormula(character, Stat.Pressure, Stat.Intelligence, Stat.Technique)},
        {(Category.Block, DuelCommand.Move),    (character, move) => CalcMove(character, move, Stat.Pressure)},

        // Shoot
        {(Category.Shoot, DuelCommand.Melee),   (character, move) => CalcFormula(character, Stat.Kick, Stat.Control, Stat.Pressure)},
        {(Category.Shoot, DuelCommand.Ranged),  (character, move) => CalcFormula(character, Stat.Kick, Stat.Control, Stat.Technique)},
        {(Category.Shoot, DuelCommand.Move),    (character, move) => CalcMove(character, move, Stat.Kick)},

        // Catch
        {(Category.Catch, DuelCommand.Melee),   (character, move) => CalcFormula(character, Stat.Agility, Stat.Physical, Stat.Pressure)},
        {(Category.Catch, DuelCommand.Ranged),  (character, move) => CalcFormula(character, Stat.Agility, Stat.Physical, Stat.Technique)},
        {(Category.Catch, DuelCommand.Move),    (character, move) => CalcMove(character, move, Stat.Agility)}
    };

    public static float GetDamage(
        Category category, 
        DuelCommand command, 
        Character character, 
        Move move,
        bool isKeeperDuel,
        bool isDirect)
    {
        float damage = 0f;
        if (damageFormulas.TryGetValue((category, command), out var formula))
            damage = formula(character, move);

        if (isKeeperDuel && character.IsKeeper)
            damage *= KEEPER_MULTIPLIER;

        if ((category == Category.Shoot && move == null) || //move is null
            (move != null && move.Trait != Trait.Long))     //move doesn't have long trait
            damage -= CalcDistanceReduction(character);

        if (isDirect)
            damage += DIRECT_BONUS;

        return damage;
    }

    public static bool IsEffective(
        Element offenseElement, 
        Element defenseElement)
    {
        int offenseIndex = (int)offenseElement;
        int defenseIndex = (int)defenseElement;

        // Compute next in cycle (with wrap-around)
        int nextIndex = (offenseIndex + 1) % System.Enum.GetValues(typeof(Element)).Length;

        return defenseIndex == nextIndex;
    }
}
