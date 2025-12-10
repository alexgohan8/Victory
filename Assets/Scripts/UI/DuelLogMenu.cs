using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Log;
using Simulation.Enums.Input;

public class DuelLogMenu : Menu
{
    [Header("References")]
    [SerializeField] private DuelLogPopup popupPrefab;
    [SerializeField] private Transform contentParent; 
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private DuelLogPopupStack popupStack;

    // Pooling & performance settings
    private readonly List<DuelLogPopup> popupPool = new List<DuelLogPopup>();
    private const int MAX_LOGS_VISIBLE = 30;
    private Coroutine populateCoroutine;

    public bool IsDuelLogMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    private void Awake()
    {
        BattleUIManager.Instance.RegisterDuelLogMenu(this);
    }

    private void Start()
    {
        // Start hidden but without disabling the GameObject
        SetVisible(false);
        base.SetInteractable(false);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterDuelLogMenu(this);
    }

    private void Update()
    {
        if (!IsInteractable()) return;
        HandleInput();
    }

    private void HandleInput()
    {
        if (IsDuelLogMenuOpen && InputManager.Instance.GetDown(CustomAction.BattleUI_CloseBattleMenu))
        {
            MenuManager.Instance.CloseMenu();
        }
    }

    // ✅ Show the menu using CanvasGroup.alpha instead of disabling
    public override void Show()
    {
        SetVisible(true);
        base.Show();

        if (populateCoroutine != null)
            StopCoroutine(populateCoroutine);

        populateCoroutine = StartCoroutine(PopulateLogCoroutine());
    }

    // ✅ Hide without disabling GameObject
    public override void Hide()
    {
        SetVisible(false);
        base.Hide();
    }

    private void SetVisible(bool visible)
    {
        if (!base.CanvasGroup) return;

        base.CanvasGroup.alpha = visible ? 1f : 0f;
        base.CanvasGroup.interactable = visible;
        base.CanvasGroup.blocksRaycasts = visible;
    }

    // ✅ Efficient coroutine-based population with pooling
    private IEnumerator PopulateLogCoroutine()
    {
        var layoutGroup = contentParent.GetComponent<VerticalLayoutGroup>();
        var fitter = contentParent.GetComponent<ContentSizeFitter>();

        // Temporarily disable layouts for performance
        if (layoutGroup) layoutGroup.enabled = false;
        if (fitter) fitter.enabled = false;

        // Deactivate all pooled popups
        foreach (var popup in popupPool)
        {
            popup.gameObject.SetActive(false);
        }

        // Retrieve recent logs
        var entries = DuelLogManager.Instance.DuelLogEntries;
        int startIndex = Mathf.Max(0, entries.Count - MAX_LOGS_VISIBLE);

        int index = 0;
        for (int i = startIndex; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry.LogLevel == LogLevel.Info) continue;

            DuelLogPopup popup;
            if (index < popupPool.Count)
            {
                popup = popupPool[index];
            }
            else
            {
                popup = Instantiate(popupPrefab, contentParent);
                popupPool.Add(popup);
            }

            popup.ShowStatic(entry);
            popup.gameObject.SetActive(true);
            index++;

            // Yield periodically to avoid frame hitching
            if (index % 10 == 0)
                yield return null;
        }

        // Re-enable layout for final rebuild
        if (layoutGroup) layoutGroup.enabled = true;
        if (fitter) fitter.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentParent);

        // Scroll to bottom next frame
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void HandleMenuOpened(Menu menu)
    {
        if (menu == this) return;
        if (!IsDuelLogMenuOpen) return;

        MenuManager.Instance.CloseMenu();
    }
}
