using Godot;
using GodotPlugins.Game;
using System;

public partial class IncreaseMobSpawnRateButton : Button
{
    [Signal]
    public delegate void IncreaseMobSpawnRateEventHandler();

    // The current mob spawn rate, initialized to 0.
    public int spawnRate = 0;

    [Export]
    private int spawnRateIncreaseCost = 20; // Cost to increase mob spawn rate.

    [Export]
    private int spawnRateInitialCost = 50; // Initial cost to increase mob spawn rate.

    private int spawnRateCost; // Current cost to increase mob spawn rate.

    private Label mobSpawnRateCost; // UI label displaying the cost.

    public override void _Ready()
    {
        // Initialize the UI elements.
        mobSpawnRateCost = GetNode<Label>("MobSpawnRateCost");
        mobSpawnRateCost.Text = "$" + spawnRateInitialCost;

        // Initialize the current cost to the initial cost.
        spawnRateCost = spawnRateInitialCost;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnButtonPressed()
    {
        GD.Print("MOB SPAWN RATE INCREASING");
        var mainNode = GetTree().Root.GetNode<main>("Main");
        int Score = mainNode.GetScore();

        // Check if the player's score is sufficient to increase the mob spawn rate.
        if (Score >= spawnRateCost)
        {
            mainNode.IncrementScoreBy(-spawnRateCost); // Decrement the player's score.
            spawnRateCost += spawnRateIncreaseCost; // Increase the cost for the next upgrade.

            var timerLabel = GetNode<Label>("MobSpawnRateLabel");
            spawnRate += 5; // Increase the mob spawn rate by 5%.
            timerLabel.Text = spawnRate.ToString() + "%"; // Update the UI label.
            mobSpawnRateCost.Text = "$" + spawnRateCost; // Update the UI cost display.

            EmitSignal(SignalName.IncreaseMobSpawnRate); // Emit a signal to notify listeners of the increase.
        }
    }
}
