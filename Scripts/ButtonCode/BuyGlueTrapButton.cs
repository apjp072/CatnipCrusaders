using Godot;
using System;

public partial class BuyGlueTrapButton : Button
{
    // Variables for glue trap properties
    private int mouseTrapCount;
    [Export] private int mouseTrapValue;         // Amount a glue trap gives you per timeout
    [Export] private int mouseTrapCost;          // Initial cost of a glue trap
    [Export] private int mouseTrapCostIncrement; // Incremental cost of a glue trap
    [Export] private int initialTimer;           // Initial timer duration for a glue trap (after how many seconds you get the money)

    // UI elements
    private Label mouseTrapCostLabel;
    private ProgressBar mouseTrapTimerBar;
    private Timer mouseTrapTimer;
    private main mainNode; // Reference to the main game node

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");

        // Initialize UI elements
        mouseTrapCostLabel = GetNode<Label>("MouseTrapCostLabel");
        mouseTrapCostLabel.Text = "$" + mouseTrapCost;

        mouseTrapTimerBar = GetNode<ProgressBar>("MouseTrapTimerBar");
        mouseTrapTimer = GetNode<Timer>("MouseTrapTimer");
        mouseTrapTimer.WaitTime = initialTimer;

        mouseTrapTimerBar.MinValue = 0;
        mouseTrapTimerBar.MaxValue = mouseTrapTimer.WaitTime;
        mouseTrapTimerBar.Value = 0; // Start empty
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update the ProgressBar to show the remaining timer
        if (mouseTrapTimer.IsStopped())
        {
            mouseTrapTimerBar.Value = 0;
        }
        else
        {
            mouseTrapTimerBar.Value = mouseTrapTimer.WaitTime - mouseTrapTimer.TimeLeft;
        }
    }

    // Called when the Buy button is pressed
    public void onButtonPress()
    {
        var score = mainNode.GetScore();
        var currentCost = mouseTrapCost + (mouseTrapCount * mouseTrapCostIncrement);
        
        // Check if the player has enough score to buy a glue trap
        if (score >= currentCost)
        {
            // Deduct the cost from the player's score
            mainNode.IncrementScoreBy(-1 * currentCost);
            
            // Increment the glue trap count
            mouseTrapCount++;

            // Update the glue trap quantity label
            var mouseTrapQuantityLabel = GetNode<Label>("MouseTrapQuantityLabel");
            mouseTrapQuantityLabel.Text = mouseTrapCount.ToString();

            // Update the glue trap cost label
            mouseTrapCostLabel.Text = "$" + FormatNumber(mouseTrapCost + (mouseTrapCount * mouseTrapCostIncrement)).ToString();

            // Start the timer if it's not already running
            if (mouseTrapTimer.TimeLeft == 0)
            {
                mouseTrapTimer.Start();
            }
        }
    }

    // Called when the glue trap timer times out
    public void OnGlueTrapTimeout()
    {
        GD.Print("Glue trap value: " + mouseTrapValue);
        
        // Increment the player's score based on the glue trap value and count
        mainNode.IncrementScoreBy(mouseTrapValue * mouseTrapCount);
    }

    // Helper function to format large numbers for display
    private string FormatNumber(int number)
    {
        if (number >= 1000)
        {
            return (number / 1000f).ToString("0.0") + "k";
        }
        else if (number >= 10000)
        {
            return (number / 10000f).ToString("0") + "k";
        }
        else if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0") + "M";
        }
        return number.ToString();
    }
}
