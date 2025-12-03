using UnityEngine;
using System.Threading.Tasks;

public class TeamComponentAppearance
{
    #region Sprite
    public Sprite TeamEmblemSprite { get; private set; }
    #endregion

    #region Address
    private string _teamEmblemAddress;
    #endregion

    #region Internal
    private Team team;
    private string defaultId = "default";
    #endregion

    public TeamComponentAppearance(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        _ = InitializeAsync(teamData);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_teamEmblemAddress)) AddressableLoader.Release(_teamEmblemAddress);
    }

    private async Task InitializeAsync(TeamData teamData)
    {
        _teamEmblemAddress = AddressableLoader.GetTeamEmblemAddress(teamData.TeamId);
        TeamEmblemSprite = await AddressableLoader.LoadAsync<Sprite>(_teamEmblemAddress);
        if (!TeamEmblemSprite) 
        {
            _teamEmblemAddress = AddressableLoader.GetTeamEmblemAddress(defaultId);
            TeamEmblemSprite = await AddressableLoader.LoadAsync<Sprite>(_teamEmblemAddress);
        }
    }

}
