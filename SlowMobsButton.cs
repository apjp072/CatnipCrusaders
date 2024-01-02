using Godot;
using System;

public partial class SlowMobsButton : Button
{
    // Signals for button events
    [Signal]
    public delegate void SlowMobsEventHandler();
    [Signal]
    public delegate void ResetMobSpeedEventHandler();

    // UI elements and timer
    private Label timerLabel;
    private Label slowMobCostLabel;
    private Label slowMobItemName;
    private Timer resetMobSpeedTimer;

    // Exported variables
    [Export]
    public int slowMobCost;
    [Export]
    public int slowMobCostIncreaseBy;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialize references to UI elements and timer
        timerLabel = GetNode<Label>("SlowMobsTimerLabel");
        slowMobCostLabel = GetNode<Label>("SlowMobsCostLabel");
        slowMobCostLabel.Text = "$" + slowMobCost;
        slowMobItemName = GetNode<Label>("SlowMobsItemName");
        resetMobSpeedTimer = GetNode<Timer>("ResetMobSpeed");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update timer label if the timer is running
        if (!resetMobSpeedTimer.IsStopped())
        {
            UpdateTimerLabel(resetMobSpeedTimer.TimeLeft);
        }
    }

    // Handle button press to slow down mobs
    private void OnSlowMobsButtonPressed()
    {
        var mainNode = GetTree().Root.GetNode<main>("Main"); // Adjust the path to your Main node
        int Score = mainNode.GetScore();

        // Check if the timer is stopped
        if (resetMobSpeedTimer.IsStopped() == true)
        {
            // Check if the player's score is sufficient to slow down mobs
            if (Score >= slowMobCost)
            {
                var slowMobCostTemp = slowMobCost;
                mainNode.IncrementScoreBy(-slowMobCost);
                slowMobCost += slowMobCostIncreaseBy;
                slowMobCostLabel.Text = "$" + slowMobCost;
                EmitSignal(SignalName.SlowMobs); // Emit signal to slow down mobs
                resetMobSpeedTimer.Start(); // Start the timer
            }
        }
    }

    // Update the timer label with the remaining time
    public void UpdateTimerLabel(double timeLeft)
    {
        var timer = GetNode<Timer>("ResetMobSpeed");
        if (timer.TimeLeft > 0.02)
        {
            timerLabel.Text = timeLeft.ToString("F0") + "s"; // Display time with 0 decimal places
            timerLabel.Show(); // Show the label
        }
        else
        {
            timerLabel.Text = resetMobSpeedTimer.WaitTime + "s";
            // Hide the label when the timer is not active
        }
    }

    // Handle the event when the timer for resetting mob speed times out
    private void OnResetMobSpeedTimeout()
    {
        var mainNode = GetTree().Root.GetNode<main>("Main"); // Adjust the path to your Main node
        mainNode.mobColor = new Color(1, 1, 1, 1);
        EmitSignal(SignalName.ResetMobSpeed); // Emit signal to reset mob speed
        UpdateTimerLabel(resetMobSpeedTimer.WaitTime);
    }

}
