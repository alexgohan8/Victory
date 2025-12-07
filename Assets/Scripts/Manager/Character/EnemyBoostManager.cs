using UnityEngine;
using System;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class EnemyBoostManager : MonoBehaviour
{
    public static EnemyBoostManager Instance { get; private set; }

    private Character character;
    private float boostFactor = 4.0f;
    private bool isActive = false;
    private bool isCharacterShoot = false;
    private List<Stat> statsToBoost = new List<Stat>
    {
        Stat.Kick,
        Stat.Control,
        Stat.Technique,
        Stat.Pressure,
        Stat.Physical,
        Stat.Agility,
        Stat.Intelligence
    };

    #region UNITY EVENTS
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        DuelEvents.OnDuelEnd += HandleDuelEnd;
        BattleEvents.OnGoalScored += HandleGoalScored;
        BattleEvents.OnAllCharactersReady += HandleAllCharactersReady;
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle; 
        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
        BattleEvents.OnShootPerformed += HandleShootPerformed;
    }

    private void OnDisable()
    {
        DuelEvents.OnDuelEnd -= HandleDuelEnd;
        BattleEvents.OnGoalScored -= HandleGoalScored;
        BattleEvents.OnAllCharactersReady -= HandleAllCharactersReady;
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;    
        BattleEvents.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
        BattleEvents.OnShootPerformed -= HandleShootPerformed;
    }
    #endregion

    /// <summary>
    /// Gives a 200% boost (x2) to the character for specific stats.
    /// </summary>
    public void TryGiveBoost()
    {
        //if hardcore mode setting
        if(!isActive)
            GiveBoost(character);
    }

    public void GiveBoost(Character character)
    {
        this.character = character;

        foreach (var stat in statsToBoost)
        {
            int currentValue = character.GetBattleStat(stat);

            int newValue = Mathf.RoundToInt(currentValue * boostFactor); // 200% increase
            int boostAmount = newValue - currentValue;

            character.ModifyBattleStat(stat, boostAmount);
        }

        isActive = true;
        DuelLogManager.Instance.AddEnemyBoostOn(character);

    }

    /// <summary>
    /// Resets the character's battle stats back to their original values.
    /// </summary>
    public void ResetBoost()
    {
        if (character == null) return;
        character.ResetBattleStats();
        DuelLogManager.Instance.AddEnemyBoostOff(character);
        character = null;
    }

    /// <summary>
    /// Handler for DuelEvents.OnDuelEnd event.
    /// </summary>
    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser)
    {
        if (isCharacterShoot) 
        {
            if (duelMode == DuelMode.Shoot && !isWinnerUser)
                ResetBoost();
        } else 
        {
            isCharacterShoot = false;
            if (duelMode == DuelMode.Shoot && loser.Character == this.character)
                ResetBoost();
        }
    }

    /// <summary>
    /// Handler for BattleEvents.OnGoalScored event.
    /// </summary>
    private void HandleGoalScored(Character scoringCharacter)
    {

    }

    private void HandleAllCharactersReady()
    {
        isActive = false;
    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (character.CharacterId == "soren" && team.TeamSide == TeamSide.Away)
        {
            this.character = character;
        }
    }

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        if (oldPhase == BattlePhase.Deadball && newPhase == BattlePhase.Battle)
            TryGiveBoost();
    }

    private void HandleShootPerformed(Character character, bool isDirect) 
    {
        if (this.character == character)
            isCharacterShoot = true;
        else
            isCharacterShoot = false;
    }
}
