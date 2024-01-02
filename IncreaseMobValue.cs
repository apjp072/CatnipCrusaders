using Godot;
using GodotPlugins.Game;
using System;

public partial class IncreaseMobValue : Button
{
    [Export]
    private double mobValueCost = 20; // The initial cost to increase mob value.

    [Export]
    private double mobValueCounter = 1; // The counter used to increase mob value cost.

    private Label mobValueCostLabel; // UI label displaying the cost.

    public override void _Ready()
    {
        var mainNode = GetTree().Root.GetNode<main>("Main");
        var mobValue = GetNode<Label>("MobValueLabel");
        mobValue.Text = mainNode.mobValue.ToString() + "x"; // Set the initial mob value display to 1x.

        mobValueCostLabel = GetNode<Label>("MobValueCost");
        mobValueCostLabel.Text = "$" + mobValueCost.ToString(); // Initialize the UI cost display.
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Additional processing can be added here if needed.
    }

    public void OnButtonPressed()
    {
        var mainNode = GetTree().Root.GetNode<main>("Main"); // Adjust the path to your Main node
        double Score = mainNode.GetScore();

        // Check if the player's score is sufficient to increase mob value.
        if (Score >= mobValueCost)
        {
            mainNode.IncrementScoreBy(-(int)mobValueCost); // Decrement the player's score.

            mainNode.mobValue *= 2; // Double the mob value.

            var mobValueLabel = GetNode<Label>("MobValueLabel");
            mobValueLabel.Text = mainNode.mobValue.ToString() + "x"; // Update the mob value display.

            mobValueCounter *= 1.25; // Increase the mob value cost counter.

            mobValueCost = Math.Floor(mobValueCost * mobValueCounter); // Calculate the new mob value cost.
            mobValueCostLabel.Text = "$" + FormatNumber((int)mobValueCost); // Update the UI cost display.
        }
    }

    private string FormatNumber(int number)
    {
        if (number >= 1000)
        {
            return (number / 1000f).ToString("0.0") + "k"; // Format numbers in thousands
        }
        else if (number >= 10000)
        {
            return (number / 10000f).ToString("0") + "k"; // Format numbers in ten thousands
        }
        else if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0") + "M"; // Format numbers in millions
        }
        return number.ToString(); // Return the original number for smaller values.
    }
}
