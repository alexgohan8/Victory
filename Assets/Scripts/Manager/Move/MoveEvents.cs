using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public static class MoveEvents
{
    public static event Action OnMoveCutsceneStart;
    public static void RaiseMoveCutsceneStart()
    {
        OnMoveCutsceneStart?.Invoke();
    }

    public static event Action OnMoveCutsceneEnd;
    public static void RaiseMoveCutsceneEnd()
    {
        OnMoveCutsceneEnd?.Invoke();
    }
}
