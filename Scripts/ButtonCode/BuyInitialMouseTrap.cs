using Godot;
using System;

public partial class BuyInitialMouseTrap : Button
{
    [Signal]
    public delegate void BuyInitialMouseTrapSignalEventHandler();

    [Export] 
	private int mouseTrapCost;
    private main mainNode;

    public bool hasMouseTrap = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Find the main game node
        mainNode = GetTree().Root.GetNode<main>("Main");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Any processing that needs to be done each frame can be added here.
    }

    // Called when the Buy button is pressed
    private void onButtonPress()
    {
        // Check if a mouse trap hasn't been bought yet
        if (!hasMouseTrap)
        {
            var score = mainNode.GetScore();
            
            // Check if the player has enough score to buy a mouse trap
            if (score >= mouseTrapCost)
            {
                // Deduct the cost from the player's score
                mainNode.IncrementScoreBy(-1 * mouseTrapCost);
                
                // Emit a signal to indicate that a mouse trap has been bought
                EmitSignal(nameof(BuyInitialMouseTrapSignalEventHandler));

                // Update the mouse trap quantity label
                GetNode<Label>("InitialMouseTrapQuantity").Text = "1";

                // Set the flag to indicate that a mouse trap has been bought
                hasMouseTrap = true;

                // Change the button's appearance to indicate ownership
                Modulate = new Color("#360000ae");
            }
        }
    }
}
