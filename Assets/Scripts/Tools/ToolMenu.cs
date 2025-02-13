using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private ToolButton toolButtonPrefab;
    [SerializeField] private Button expandButton;

    private List<ToolButton> activeButtons = new List<ToolButton>();

    private void Awake()
    {
        G.ToolMenu = this;
    }

    private void Start()
    {
        expandButton.onClick.AddListener(() => ChangeExpand());
        CreateButton(() =>
        {
            G.BuildSystem.SetMenuActive(true);
        },
        "Open buildings menu"
        );
    }

    private void OnDestroy()
    {
        expandButton.onClick.RemoveAllListeners();
        //ваще это нафиг не надо, но пофиг
        foreach (var button in activeButtons)
            button.Button.onClick.RemoveAllListeners();
    }

    public void SetMenuActive(bool state) => menuParent.gameObject.SetActive(state);
    public void SetMenuExpand(bool state) => buttonsContainer.gameObject.SetActive(state);

    private void ChangeExpand() => SetMenuExpand(!buttonsContainer.gameObject.activeSelf);
    private void CreateButton(Action action, string title)
    {
        var btn = Instantiate(toolButtonPrefab, buttonsContainer);
        btn.Button.onClick.AddListener(() =>
        {
            G.ToolMenu.SetMenuExpand(false);
            action?.Invoke();
        });
        btn.Title.text = title;

        activeButtons.Add(btn);
    }
}
