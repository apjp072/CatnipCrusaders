using Godot;
using System;

public partial class BuyTheVoidButton : Button
{
    // Variables for The Void properties
    private int theVoidCount;
    [Export] private int theVoidValue;          // Amount The Void gives you per timeout
    [Export] private int theVoidCost;           // Initial cost of The Void
    [Export] private int theVoidCostIncrement;  // Incremental cost of The Void
    [Export] private int initialTimer; 			//timer per getting theVoidValue

    // UI elements
    private Label theVoidCostLabel;
    private ProgressBar theVoidTimerBar;
    private Timer theVoidTimer;
    private main mainNode; // Reference to the main game node

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");

        // Initialize UI elements
        theVoidCostLabel = GetNode<Label>("theVoidCostLabel");
        theVoidCostLabel.Text = "$" + FormatNumber(theVoidCost);

        theVoidTimerBar = GetNode<ProgressBar>("theVoidTimerBar");
        theVoidTimer = GetNode<Timer>("theVoidTimer");
        theVoidTimer.WaitTime = initialTimer;

        theVoidTimerBar.MinValue = 0;
        theVoidTimerBar.MaxValue = theVoidTimer.WaitTime;
        theVoidTimerBar.Value = 0; // Start empty
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update ProgressBar
        if (theVoidTimer.IsStopped())
        {
            theVoidTimerBar.Value = 0;
        }
        else
        {
            theVoidTimerBar.Value = theVoidTimer.WaitTime - theVoidTimer.TimeLeft;
        }
    }

    // Called when the Buy button is pressed
    public void onButtonPress()
    {
        var score = mainNode.GetScore();
        var currentCost = theVoidCost + (theVoidCount * theVoidCostIncrement);
        
        // Check if the player has enough score to buy The Void
        if (score >= currentCost)
        {
            // Deduct the cost from the player's score
            mainNode.IncrementScoreBy(-1 * currentCost);
            
            // Increment The Void count
            theVoidCount++;

            // Update The Void quantity label
            var theVoidQuantityLabel = GetNode<Label>("theVoidQuantityLabel");
            theVoidQuantityLabel.Text = theVoidCount.ToString();

            // Update The Void cost label
            theVoidCostLabel.Text = "$" + FormatNumber(theVoidCost + (theVoidCount * theVoidCostIncrement)).ToString();

            // Start the timer if it's not already running
            if (theVoidTimer.TimeLeft == 0)
            {
                theVoidTimer.Start();
            }
        }
    }

    // Called when The Void timer times out
    public void OnTheVoidTimeout()
    {
        GD.Print("The Void value: " + theVoidValue);
        
        // Increment the player's score based on The Void value and count
        mainNode.IncrementScoreBy(theVoidValue * theVoidCount);
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
        else if (number >= 1000000) //millions
        {
            return (number / 1000000f).ToString("0") + "M";
        }
        return number.ToString();
    }
}
