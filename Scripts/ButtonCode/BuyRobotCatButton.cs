using Godot;
using System;

public partial class BuyRobotCatButton : Button
{
    // Variables for Robot Cat properties
    private int robotCatCount;
    [Export] private int robotCatValue;         // Amount a Robot Cat gives you per timeout
    [Export] private int robotCatCost;          // Initial cost of a Robot Cat
    [Export] private int robotCatCostIncrement; // Incremental cost of a Robot Cat
    [Export] private int initialTimer;

    // UI elements
    private Label robotCatCostLabel;
    private ProgressBar robotCatTimerBar;
    private Timer robotCatTimer;
    private main mainNode; // Reference to the main game node

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");

        // Initialize UI elements
        robotCatCostLabel = GetNode<Label>("robotCatCostLabel");
        robotCatCostLabel.Text = "$" + FormatNumber(robotCatCost);

        robotCatTimerBar = GetNode<ProgressBar>("robotCatTimerBar");
        robotCatTimer = GetNode<Timer>("robotCatTimer");
        robotCatTimer.WaitTime = initialTimer;

        robotCatTimerBar.MinValue = 0;
        robotCatTimerBar.MaxValue = robotCatTimer.WaitTime;
        robotCatTimerBar.Value = 0; // Start empty
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update ProgressBar
        if (robotCatTimer.IsStopped())
        {
            robotCatTimerBar.Value = 0;
        }
        else
        {
            robotCatTimerBar.Value = robotCatTimer.WaitTime - robotCatTimer.TimeLeft;
        }
    }

    // Called when the Buy button is pressed
    public void onButtonPress()
    {
        var score = mainNode.GetScore();
        var currentCost = robotCatCost + (robotCatCount * robotCatCostIncrement);
        
        // Check if the player has enough score to buy a Robot Cat
        if (score >= currentCost)
        {
            // Deduct the cost from the player's score
            mainNode.IncrementScoreBy(-1 * currentCost);
            
            // Increment the Robot Cat count
            robotCatCount++;

            // Update the Robot Cat quantity label
            var robotCatQuantityLabel = GetNode<Label>("robotCatQuantityLabel");
            robotCatQuantityLabel.Text = robotCatCount.ToString();

            // Update the Robot Cat cost label
            robotCatCostLabel.Text = "$" + FormatNumber(robotCatCost + (robotCatCount * robotCatCostIncrement)).ToString();

            // Start the timer if it's not already running
            if (robotCatTimer.TimeLeft == 0)
            {
                robotCatTimer.Start();
            }
        }
    }

    // Called when the Robot Cat timer times out
    public void OnRobotCatTimeout()
    {
        GD.Print("Robot Cat value: " + robotCatValue);
        
        // Increment the player's score based on the Robot Cat value and count
        mainNode.IncrementScoreBy(robotCatValue * robotCatCount);
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
