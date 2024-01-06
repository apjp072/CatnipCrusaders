using Godot;
using Godot.Collections;
using System.IO;
using System;

public partial class ItemShopDropDown : Control
{
    [Signal]
    public delegate void SlowMobsToMainEventHandler();

    [Signal]
    public delegate void ResetMobSpeedToMainEventHandler();

    [Signal]
    public delegate void IncreaseMobSpawnRateToMainEventHandler();

    [Signal]
    public delegate void CloseOtherDropdownsEventHandler();

    [Signal]
    public delegate void BuyInitialMouseTrapToControlEventHandler();

    [Signal]
    public delegate void BuyInitialCatnipToControlEventHandler();

    private TextureRect textureRect;
    private SlowMobsButton slowMobsButton;
    private IncreaseMaxMobsButton increaseMaxMobsButton;
    private IncreaseMobSpawnRateButton increaseMobSpawnRateButton;
    private IncreaseMobValue increaseMobValueButton;
    private BuyInitialMouseTrap buyInitialMouseTrap;
    private BuyInitialCatnip buyInitialCatnip;

    public string basePath = ProjectSettings.GlobalizePath("user://");
    public string fileName = "savegameItemShop.json";
    string filePath;

    public void SaveGame()
    {
        GD.Print("SAVING GAME!!!");

        // Save data related to Slow Mobs Button
        int slowMobCost = slowMobsButton.slowMobCost;
        int slowMobCostIncreaseBy = slowMobsButton.slowMobCostIncreaseBy;

        Dictionary data = new Dictionary();
        data.Add("slowMobCost", slowMobCost);
        data.Add("slowMobCostIncreaseBy", slowMobCostIncreaseBy);

        string stringData = Json.Stringify(data);

        try
        {
            File.WriteAllText(filePath, stringData);
        }
        catch (SystemException e)
        {
            GD.Print(e);
        }
    }

    public void LoadGame()
    {
        string loadedData;

        if (!File.Exists(filePath)) return;

        try
        {
            loadedData = File.ReadAllText(filePath);
        }
        catch (SystemException e)
        {
            GD.Print(e);
            loadedData = "";
        }

        Json jsonLoader = new Json();
        Error error = jsonLoader.Parse(loadedData);

        if (error != Error.Ok)
        {
            GD.Print(error);
            return;
        }

        Dictionary loadedDataDict = (Dictionary)jsonLoader.Data;

        // Load data related to Slow Mobs Button
        slowMobsButton.slowMobCost = (int)loadedDataDict["slowMobCost"];
        slowMobsButton.slowMobCostIncreaseBy = (int)loadedDataDict["slowMobCostIncreaseBy"];
    }

    public override void _Ready()
    {
        textureRect = GetNode<TextureRect>("TextureRect");
        slowMobsButton = textureRect.GetNode<SlowMobsButton>("SlowMobsButton");
        increaseMaxMobsButton = textureRect.GetNode<IncreaseMaxMobsButton>("IncreaseMaxMobsButton");
        increaseMobSpawnRateButton = textureRect.GetNode<IncreaseMobSpawnRateButton>("IncreaseMobSpawnRateButton");
        increaseMobValueButton = textureRect.GetNode<IncreaseMobValue>("IncreaseMobValue");
        buyInitialMouseTrap = textureRect.GetNode<BuyInitialMouseTrap>("BuyInitialMouseTrap");
        buyInitialCatnip = textureRect.GetNode<BuyInitialCatnip>("BuyInitialCatnip");

        filePath = Path.Join(basePath, fileName);

        Visible = false;
        this.MouseFilter = MouseFilterEnum.Pass;
    }

    public override void _Process(double delta)
    {
        // Additional processing can be added here if needed.
    }

    public void SlowMobs()
    {
        EmitSignal(SignalName.SlowMobsToMain);
    }

    public void ResetMobSpeed()
    {
        EmitSignal(SignalName.ResetMobSpeedToMain);
    }

    public void IncreaseMobSpawnRate()
    {
        EmitSignal(SignalName.IncreaseMobSpawnRateToMain);
    }

    public void MouseTrap()
    {
        EmitSignal(SignalName.BuyInitialMouseTrapToControl);
    }

    public void Catnip()
    {
        EmitSignal(SignalName.BuyInitialCatnipToControl);
    }
}
