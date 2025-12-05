using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class FaderMoveCutscene : MonoBehaviour
{
    [Header("Visibility Control")]
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        MoveEvents.OnMoveCutsceneStart += HandleCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd += HandleCutsceneEnd;
    }

    private void OnDestroy()
    {
        MoveEvents.OnMoveCutsceneStart -= HandleCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleCutsceneEnd;
    }

    private void HandleCutsceneStart()
    {
        canvasGroup.alpha = 0f;
    }

    private void HandleCutsceneEnd()
    {
        canvasGroup.alpha = 1f;
    }

}
