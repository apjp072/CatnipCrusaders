using Godot;
using System;

public partial class BuyNanoSwarmButton : Button
{
    // Variables for Nano Swarm properties
    private int nanoSwarmCount;
    [Export] private int nanoSwarmValue;         // Amount a mouse trap gives you per timeout
    [Export] private int nanoSwarmCost;          // Initial cost of a mouse trap
    [Export] private int nanoSwarmCostIncrement; // Incremental cost of a mouse trap
    [Export] private int initialTimer;

    // UI elements
    private Label nanoSwarmCostLabel;
    private ProgressBar nanoSwarmTimerBar;
    private Timer nanoSwarmTimer;
    private main mainNode; // Reference to the main game node

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");

        // Initialize UI elements
        nanoSwarmCostLabel = GetNode<Label>("nanoSwarmCostLabel");
        nanoSwarmCostLabel.Text = "$" + FormatNumber(nanoSwarmCost);

        nanoSwarmTimerBar = GetNode<ProgressBar>("nanoSwarmTimerBar");
        nanoSwarmTimer = GetNode<Timer>("nanoSwarmTimer");
        nanoSwarmTimer.WaitTime = initialTimer;

        nanoSwarmTimerBar.MinValue = 0;
        nanoSwarmTimerBar.MaxValue = nanoSwarmTimer.WaitTime;
        nanoSwarmTimerBar.Value = 0; // Start empty
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update ProgressBar
        if (nanoSwarmTimer.IsStopped())
        {
            nanoSwarmTimerBar.Value = 0;
        }
        else
        {
            nanoSwarmTimerBar.Value = nanoSwarmTimer.WaitTime - nanoSwarmTimer.TimeLeft;
        }
    }

    // Called when the Buy button is pressed
    public void onButtonPress()
    {
        var score = mainNode.GetScore();
        var currentCost = nanoSwarmCost + (nanoSwarmCount * nanoSwarmCostIncrement);
        
        // Check if the player has enough score to buy a mouse trap
        if (score >= currentCost)
        {
            // Deduct the cost from the player's score
            mainNode.IncrementScoreBy(-1 * currentCost);
            
            // Increment the mouse trap count
            nanoSwarmCount++;

            // Update the mouse trap quantity label
            var nanoSwarmQuantityLabel = GetNode<Label>("nanoSwarmQuantityLabel");
            nanoSwarmQuantityLabel.Text = nanoSwarmCount.ToString();

            // Update the mouse trap cost label
            nanoSwarmCostLabel.Text = "$" + FormatNumber(nanoSwarmCost + (nanoSwarmCount * nanoSwarmCostIncrement)).ToString();

            // Start the timer if it's not already running
            if (nanoSwarmTimer.TimeLeft == 0)
            {
                nanoSwarmTimer.Start();
            }
        }
    }

    // Called when the Nano Swarm timer times out
    public void OnNanoSwarmTimeout()
    {
        GD.Print("Mouse trap value: " + nanoSwarmValue);
        
        // Increment the player's score based on the mouse trap value and count
        mainNode.IncrementScoreBy(nanoSwarmValue * nanoSwarmCount);
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
