using Godot;
using GodotPlugins.Game;
using System;

public partial class IncreaseMaxMobsButton : Button
{
	// Called when the node enters the scene tree for the first time.

	[Export]
	private int maxMobIncreaseCost = 5;
	[Export]
	private int increaseBy = 5;
	private Label maxMobsCostLabel;
	private Label maxMobsQuantityLabel;
	public override void _Ready()
	{
		var mainNode = GetTree().Root.GetNode<main>("Main");
		maxMobsQuantityLabel = GetNode<Label>("MaxMobsLabel");
		maxMobsQuantityLabel.Text = mainNode.MaxMobs.ToString();
		maxMobsCostLabel = GetNode<Label>("MaxMobsCost");
		maxMobsCostLabel.Text = "$" + maxMobIncreaseCost;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnButtonPressed()
	{
		var mainNode = GetTree().Root.GetNode<main>("Main");
		int Score = mainNode.GetScore();

		if (Score >= maxMobIncreaseCost)
		{
			mainNode.IncrementScoreBy(-maxMobIncreaseCost); //decrement score

			mainNode.MaxMobs += 1;

			mainNode.UpdateMobsCollectedLabel();

			maxMobsQuantityLabel.Text = mainNode.MaxMobs.ToString();

			maxMobIncreaseCost += increaseBy;

			maxMobsCostLabel.Text = "$" + maxMobIncreaseCost;
			
		}

	}
}

