using Godot;
using System;

public partial class BuyPredatorUAVButton : Button
{
    // Variables for Predator UAV properties
    private int predatorUAVCount;
    [Export] private int predatorUAVValue;         // Amount a Predator UAV gives you per timeout
    [Export] private int predatorUAVCost;          // Initial cost of a Predator UAV
    [Export] private int predatorUAVCostIncrement; // Incremental cost of a Predator UAV
    [Export] private int initialTimer;

    // UI elements
    private Label predatorUAVCostLabel;
    private ProgressBar predatorUAVTimerBar;
    private Timer predatorUAVTimer;
    private main mainNode; // Reference to the main game node

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");

        // Initialize UI elements
        predatorUAVCostLabel = GetNode<Label>("predatorUAVCostLabel");
        predatorUAVCostLabel.Text = "$" + FormatNumber(predatorUAVCost);

        predatorUAVTimerBar = GetNode<ProgressBar>("predatorUAVTimerBar");
        predatorUAVTimer = GetNode<Timer>("predatorUAVTimer");
        predatorUAVTimer.WaitTime = initialTimer;

        predatorUAVTimerBar.MinValue = 0;
        predatorUAVTimerBar.MaxValue = predatorUAVTimer.WaitTime;
        predatorUAVTimerBar.Value = 0; // Start empty
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update ProgressBar
        if (predatorUAVTimer.IsStopped())
        {
            predatorUAVTimerBar.Value = 0;
        }
        else
        {
            predatorUAVTimerBar.Value = predatorUAVTimer.WaitTime - predatorUAVTimer.TimeLeft;
        }
    }

    // Called when the Buy button is pressed
    public void onButtonPress()
    {
        var score = mainNode.GetScore();
        var currentCost = predatorUAVCost + (predatorUAVCount * predatorUAVCostIncrement);
        
        // Check if the player has enough score to buy a Predator UAV
        if (score >= currentCost)
        {
            // Deduct the cost from the player's score
            mainNode.IncrementScoreBy(-1 * currentCost);
            
            // Increment the Predator UAV count
            predatorUAVCount++;

            // Update the Predator UAV quantity label
            var predatorUAVQuantityLabel = GetNode<Label>("predatorUAVQuantityLabel");
            predatorUAVQuantityLabel.Text = predatorUAVCount.ToString();

            // Update the Predator UAV cost label
            predatorUAVCostLabel.Text = "$" + FormatNumber(predatorUAVCost + (predatorUAVCount * predatorUAVCostIncrement)).ToString();

            // Start the timer if it's not already running
            if (predatorUAVTimer.TimeLeft == 0)
            {
                predatorUAVTimer.Start();
            }
        }
    }

    // Called when the Predator UAV timer times out
    public void OnPredatorUAVTimeout()
    {
        GD.Print("Predator UAV value: " + predatorUAVValue);
        
        // Increment the player's score based on the Predator UAV value and count
        mainNode.IncrementScoreBy(predatorUAVValue * predatorUAVCount);
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
