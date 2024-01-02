using Godot;
using System;

public partial class hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    [Signal]
    public delegate void SlowMobsEventHandler();

    public main mainNode;

    public TextureRect youWonScreen; // Screen for the "You Won" message
    public TextureRect StartGameScreen; // Screen for starting the game

    public bool isContinuing; // Flag to indicate if the game is continuing from a saved state

    public override void _Ready()
    {
        mainNode = GetTree().Root.GetNode<main>("Main"); // Get reference to the main game node
        youWonScreen = GetNode<TextureRect>("YouWon");
        youWonScreen.Visible = false; // Initialize the "You Won" screen as invisible
        StartGameScreen = GetNode<TextureRect>("StartGame");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        GetNode<Timer>("MessageTimer").Start();
    }

    public void ShowGameOver()
    {
        youWonScreen.Visible = true; // Show the "You Won" screen
        mainNode.GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("WinningMusic").Play();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = "$" + score.ToString();

        if (score > 10000000)
        {
            ShowGameOver();
        }

        mainNode.SaveGame();
    }

    private void OnStartButtonPressed()
    {
        StartGameScreen.Visible = false; // Hide the start game screen
        GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.StartGame); // Emit the StartGame signal to begin the game
    }

    private void OnMessageTimerTimeout()
    {
        GetNode<Label>("Message").Hide();
    }

    private void OnSlowMobsButtonPressed()
    {
        GD.Print("Button Pressed");
        var mainNode = GetTree().Root.GetNode<main>("Main"); // Adjust the path to your Main node
        var timer = mainNode.GetNode<Timer>("ResetMobSpeed");

        if (timer.IsStopped() == true)
        {
            GD.Print(mainNode);
            int Score = mainNode.GetScore();
            GD.Print(Score);
            if (Score >= 2)
            {
                GD.Print("SCORE IS MORE THAN 2!");
                mainNode.IncrementScoreBy(-2);
                EmitSignal(SignalName.SlowMobs); // Emit the SlowMobs signal to slow down enemies
            }
            timer.Start();
        }
    }

    private void OnRespawn()
    {
        youWonScreen.Visible = false; // Hide the "You Won" screen
        mainNode.NewGame();
        mainNode.totalScore = 0;
        mainNode.SaveGame();
        GetNode<AudioStreamPlayer>("WinningMusic").Stop();
        mainNode.GetNode<AudioStreamPlayer>("Music").Play();
    }

    private void OnContinue()
    {
        youWonScreen.Visible = false; // Hide the "You Won" screen
        GetNode<AudioStreamPlayer>("WinningMusic").Stop();
        mainNode.GetNode<AudioStreamPlayer>("Music").Play();
    }
}
