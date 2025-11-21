using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform menubackground;
    [SerializeField] private List<GameObject> interactives = new List<GameObject>();
    [SerializeField] private Button showMenubttn;

    [SerializeField] private Button StartBttn;
    [SerializeField] private Button OptionsBttn;
    [SerializeField] private Button ExitBttn;

    public List<GameObject> Interactives { get => interactives; }

    public Transform MenuBackground { get => menubackground; }

    public Button ShowMenuBttn { get => showMenubttn; }

    private void OnEnable()
    {
        if (StartBttn != null)
            StartBttn.onClick.AddListener(MenuManager.Instance.StartGame);
        if (OptionsBttn != null)
            OptionsBttn.onClick.AddListener(MenuManager.Instance.OpenSetting);
        if (ExitBttn != null)
            ExitBttn.onClick.AddListener(MenuManager.Instance.QuitGame);

    }

    private void OnDisable()
    {
        if (StartBttn != null)
            StartBttn.onClick.RemoveListener(MenuManager.Instance.StartGame);
        if (OptionsBttn != null)
            OptionsBttn.onClick.RemoveListener(MenuManager.Instance.OpenSetting);
        if (ExitBttn != null)
            ExitBttn.onClick.RemoveListener(MenuManager.Instance.QuitGame);
    }




}
