using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RomMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Transform romButtonParent;
    [SerializeField] private Button romButtonPrefab;
    [SerializeField] private CPU chip8Runner;

    private void Start()
    {
        ShowMenu();
        CreateRomButtons();
    }

    private void CreateRomButtons()
    {
        TextAsset[] roms = Resources.LoadAll<TextAsset>("Roms");
        Array.Sort(roms, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        
        foreach (TextAsset rom in roms)
        {
            Button button = Instantiate(romButtonPrefab, romButtonParent);

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            label.text = rom.name;

            button.onClick.AddListener(() =>
            {
                StartRom(rom);
            });
        }

    }

    private void StartRom(TextAsset rom)
    {
        menuPanel.SetActive(false);

        chip8Runner.LoadRom(rom.bytes);
        chip8Runner.SetPaused(false);
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        chip8Runner.SetPaused(true);
    }
}