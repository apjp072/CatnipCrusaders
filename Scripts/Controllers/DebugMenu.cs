using Godot;
using GodotPlugins.Game;
using System;

//For debugging purposes if the player gets stuck or something goes wrong
public partial class DebugMenu : Control
{
    private main mainNode; 
    private TextureRect textureRect; 
    private Player player; 
    public override void _Ready()
    {

        mainNode = GetTree().Root.GetNode<main>("Main");
        textureRect = GetNode<TextureRect>("TextureRect");
        textureRect.Visible = false;


        player = mainNode.GetNode<Player>("Player");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    
    }

    // Toggle the visibility of the debug menu on button press
    private void onButtonPress()
    {
        textureRect.Visible = !textureRect.Visible;
    }

    // Kill all mobs in the game
    private void onKillAllMobs()
    {
        mainNode.KillAllMobs();
    }

    // Reset the player's position
    private void onResetPlayerPosition()
    {
        mainNode.ResetPlayerPosition();
    }

    // Add a large amount of money to the player's score for kicks and gigs
    private void onGetMonies()
    {
        mainNode.IncrementScoreBy(9999999);
    }
}
