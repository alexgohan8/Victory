using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class BattleScoreboard : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTextHome;
    [SerializeField] private TMP_Text scoreTextAway;
    [SerializeField] private Image teamEmblemHome;
    [SerializeField] private Image teamEmblemAway;
    [SerializeField] private TMP_Text teamNameHome;
    [SerializeField] private TMP_Text teamNameAway;
    [SerializeField] private CanvasGroup canvasGroup;

    private Dictionary<TeamSide, TMP_Text> scoreTextDict;
    private Dictionary<TeamSide, Image> teamEmblemDict;
    private Dictionary<TeamSide, TMP_Text> teamNameDict;

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterScoreboard(this);

        scoreTextDict = new Dictionary<TeamSide, TMP_Text>
        {
            { TeamSide.Home, scoreTextHome },
            { TeamSide.Away, scoreTextAway }
        };

        teamEmblemDict = new Dictionary<TeamSide, Image>
        {
            { TeamSide.Home, teamEmblemHome },
            { TeamSide.Away, teamEmblemAway }
        };

        teamNameDict = new Dictionary<TeamSide, TMP_Text>
        {
            { TeamSide.Home, teamNameHome },
            { TeamSide.Away, teamNameAway }
        };
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterScoreboard(this);
    }

    public void SetTeam(Team team)
    {
        teamEmblemDict[team.TeamSide].sprite = team.TeamEmblemSprite;
        teamNameDict[team.TeamSide].text = team.TeamName;
    }

    public void UpdateScoreDisplay(Team team, int scoreValue)
    {
        scoreTextDict[team.TeamSide].text = scoreValue.ToString();
    }

    public void Reset() 
    {
        scoreTextDict[TeamSide.Home].text = "0";
        scoreTextDict[TeamSide.Away].text = "0";
    }

}
