using Godot;
using System;
using Godot.Collections;
using System.IO;

public partial class main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    [Signal]
    public delegate void SaveGameSignalEventHandler();

    private Timer _myTimer;
    private hud _hud;
    private Control _ItemShopDropDown;
    private int _score;
    private float _speedMultiplier = 1;
    public Color mobColor = new Color(1, 1, 1, 1);
    private int mobCount = 0;
    public int MaxMobs = 1;

    private int MobsCollected { get; set; }

    private ProgressBar mobsCollectedProgressBar;
    private Label mobsCollectedProgressBarLabel;
    public int mobValue = 1;
    public int mobClockCount = 1;
    private double initialMobSpawnRate;

    public string basePath = ProjectSettings.GlobalizePath("user://");
    public string fileName = "savegameMain.json";
    string filePath;

    public int totalScore = 0;

    // Dictionary gameData;

    public void SaveGame()
    {
        Dictionary data = new Dictionary();
        data.Add("_score", _score);
        data.Add("totalScore", totalScore);

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
        _score = (int)loadedDataDict["_score"];
        totalScore = (int)loadedDataDict["totalScore"];

        _score = totalScore;
        GetNode<hud>("HUD").UpdateScore(_score);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _hud = GetNode<hud>("HUD"); // Adjust the path to your HUD node
        Timer mobTimer = GetNode<Timer>("MobTimer");
        initialMobSpawnRate = mobTimer.WaitTime;
        MobsCollected = 0;
        mobsCollectedProgressBar = GetNode<ProgressBar>("MobsCollectedProgressBar");
        mobsCollectedProgressBarLabel = mobsCollectedProgressBar.GetNode<Label>("MobsCollectedProgressBarLabel");
        UpdateMobsCollectedLabel();

        filePath = Path.Join(basePath, fileName);

        var foreground = GetNode<TextureRect>("Foreground");
        foreground.ZIndex = 3;

        var shopsController = GetNode<ShopsController>("ShopsController");
        shopsController.ZIndex = 5; // on top of progress bar and foreground

        NewGame();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Additional processing can be added here if needed.
    }

    public int GetScore()
    {
        return _score;
    }

    public void SetScore(int Score)
    {
        _score = Score;
    }

    public void GameOver()
    {
        GD.Print("GAME OVER!?");
        GetNode<hud>("HUD").ShowGameOver();
    }

    public void NewGame()
    {
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
        GD.Print("HERE!!!");
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);
        GetNode<Timer>("StartTimer").Start();
        LoadGame();
    }

    private void OnMobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();

        Callable mobIsDeadCallable = new Callable(this, nameof(MobIsDead));
        mob.Connect("MobDied", mobIsDeadCallable);

        bool spawnLeft = GD.Randi() % 2 == 0;
        Vector2 spawnPosition = spawnLeft ? GetNode<Marker2D>("LeftSpawnPoint").Position : GetNode<Marker2D>("RightSpawnPoint").Position;
        mob.Position = spawnPosition;
        if (spawnPosition == GetNode<Marker2D>("RightSpawnPoint").Position)
        {
            mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
        }

        float initialSpeed = (float)GD.RandRange(150.0, 250.0);
        Vector2 initialVelocity = new Vector2(spawnLeft ? initialSpeed : -initialSpeed, 0);
        mob.LinearVelocity = initialVelocity;

        mob.ChangeColor(mobColor);
        GD.Print("ON MAKE MOBS MOB COLOR:" + mobColor);
        mob.speedMultiplier = _speedMultiplier;

        mob.SetFloor(GetNode<CollisionObject2D>("Floor"));
        mob.SetPlayer(GetNode<CharacterBody2D>("Player"));
        mob.ZIndex = 2;
        AddChild(mob);
        mobCount += 1;
    }

    private void IncrementScore()
    {
        _score++;
        GetNode<hud>("HUD").UpdateScore(_score);
    }

    public void IncrementScoreBy(int amount)
    {
        if (amount > 0)
        {
            totalScore += amount;
        }
        _score += amount;
        GetNode<hud>("HUD").UpdateScore(_score);
    }

    public void IncrementScoreCollectionBox()
    {
        _score += MobsCollected * mobValue;
        totalScore += MobsCollected * mobValue;
        GetNode<hud>("HUD").UpdateScore(_score);
    }

    public void ResetPlayerPosition()
    {
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Position = startPosition.Position;
    }

    public void MobIsDead()
    {
        int action = (int)(GD.Randi() % 2);
        switch (action)
        {
            case 0: GetNode<AudioStreamPlayer>("CatMeow1").Play(); break;
            case 1: GetNode<AudioStreamPlayer>("CatMeow2").Play(); break;
        }
    }

    public void KillAllMobs()
    {
        var mobs = GetTree().GetNodesInGroup("mobs");
        foreach (Node mob in mobs)
        {
            mob.QueueFree();
        }
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
    }

    private void OnResetMobSpeedTimeout()
    {
        _speedMultiplier = 1;
    }

    private void OnSlowMobs()
    {
        _speedMultiplier *= 0.5f;
        mobColor = new Color(0, 1, 0, 0.5f);
    }

    private void IncreaseMobSpawnRate()
    {
        Timer mobTimer = GetNode<Timer>("MobTimer");
        mobClockCount += 1;
        mobTimer.WaitTime = initialMobSpawnRate * Math.Pow(0.5, mobClockCount / 100.0);
        GD.Print("Mob spawn rate: " + mobClockCount, "MobTimer Wait Time!!!!!!!! " + mobTimer.WaitTime);
    }

    public int GetMobsCollected()
    {
        return MobsCollected;
    }

    public void ResetMobsCollected()
    {
        if (MobsCollected != 0)
        {
            GetNode<AudioStreamPlayer>("CashRegisterSound").Play();
        }

        MobsCollected = 0;
        UpdateMobsCollectedLabel();
    }

    public void IncreaseMobsCollected()
    {
        MobsCollected++;
        GD.Print("Mobs collected now: " + MobsCollected);
        UpdateMobsCollectedLabel();
    }

    public void UpdateMobsCollectedLabel()
    {
        mobsCollectedProgressBar.MaxValue = MaxMobs;
        mobsCollectedProgressBar.Value = MobsCollected;
        mobsCollectedProgressBarLabel.Text = $"{MobsCollected}/{MaxMobs}";

        float percentage = (float)MobsCollected / MaxMobs * 100;
        float hue = (100 - percentage) / 365;
        Color mobsCollectedColorValue = Color.FromHsv(hue, 1, 1);
        mobsCollectedProgressBar.SelfModulate = mobsCollectedColorValue;
    }
}
