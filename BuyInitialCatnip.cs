using Godot;
using System;

public partial class BuyInitialCatnip : Button
{
    [Signal]
    public delegate void BuyInitialCatnipSignalEventHandler();

    [Export] private int catnipCost;
    private main mainNode;

    private bool hasCatnip = false;

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
        // Check if the parent node (BuyInitialMouseTrap) has a mouse trap
        bool hasMouseTrap = GetParent().GetNode<BuyInitialMouseTrap>("BuyInitialMouseTrap").hasMouseTrap;
        
        // Check if catnip hasn't been bought yet and if there's a mouse trap
        if (!hasCatnip && hasMouseTrap)
        {
            var score = mainNode.GetScore();
            
            // Check if the player has enough score to buy catnip
            if (score >= catnipCost)
            {
                // Deduct the cost from the player's score
                mainNode.IncrementScoreBy(-1 * catnipCost);
                
                // Emit a signal to indicate that catnip has been bought
                EmitSignal(nameof(BuyInitialCatnipSignalEventHandler));

                // Update the catnip quantity label
                GetNode<Label>("InitialCatnipQuantity").Text = "1";

                // Set the flag to indicate that catnip has been bought
                hasCatnip = true;

                // Change the button's appearance to indicate ownership
                Modulate = new Color("#360000ae");
            }
        }
    }
}
