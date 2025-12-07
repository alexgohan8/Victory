using UnityEngine;
using Simulation.Enums.Duel;
using Simulation.Enums.Character;

public class BallComponentTravelParticle : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private ParticleSystem energyParticle;
    private Vector3? travelDirection;
    private Element? currentElement;

    private void OnEnable()
    {
        BallEvents.OnTravelStart += HandleTravelStart;
        BallEvents.OnTravelCancel += HandleTravelCancel;
        BallEvents.OnTravelEnd += HandleTravelEnd;
    }

    private void OnDisable()
    {
        BallEvents.OnTravelStart -= HandleTravelStart;
        BallEvents.OnTravelCancel -= HandleTravelCancel;
        BallEvents.OnTravelEnd -= HandleTravelEnd;
    }

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
        energyParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleTravelStart(Vector3 startPosition)
    {
        travelDirection = (startPosition - transform.position).normalized;
    }

    private void HandleTravelCancel()
    {
        StopParticle();
        Reset();
    }

    private void HandleTravelEnd(Vector3 endPosition)
    {
        StopParticle();
        Reset();
    }

    public void TryPlayParticle(Move move)
    {
        if (move == null) 
        {
            //StopParticle();
            return;
        }

        currentElement = move.Element;
        StartParticle();
    }

    private void StartParticle()
    {
        if(!travelDirection.HasValue || !currentElement.HasValue) return;

        energyParticle.transform.forward = -travelDirection.Value;
        SetElementColor(currentElement.Value);
        energyParticle.Play(true);
        AudioManager.Instance.PlaySfxLoop("sfx-ball_energy");
    }

    private void StopParticle()
    {
        energyParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        AudioManager.Instance.StopSfxLoop("sfx-ball_energy");
    }

    private void Reset()
    {
        travelDirection = null;
        currentElement = null;
    }

    private void SetElementColor(Element element)
    {
        Color newColor = ColorManager.GetElementColor(element);
        var main = energyParticle.main;
        main.startColor = newColor;
    }

}
