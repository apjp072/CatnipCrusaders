using Godot;
using System;


//Adds money on collision depending on how many mice the player has collected
public partial class RatCollectionBox : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			GD.Print("PLayer is here!!!");
			var mainNode = GetTree().Root.GetNode<main>("Main");
			GD.Print("CALLING INCREMENT SCORE: Mobs Collected: " + mainNode.GetMobsCollected());
			mainNode.IncrementScoreCollectionBox();
			GD.Print("Mobs Collected: " + mainNode.GetMobsCollected());
			mainNode.ResetMobsCollected();

			
		}

	}
}
