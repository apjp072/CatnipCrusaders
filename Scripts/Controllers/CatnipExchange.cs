using Godot;
using System;

public partial class CatnipExchange : Control
{
	[Signal]
	public delegate void CloseOtherDropdownsEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var texture = GetNode<TextureRect>("CatNipExchangeTexture");
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnButtonPressed()
	{
		var texture = GetNode<TextureRect>("CatNipExchangeTexture");
		var passiveIncomeButton = GetNode<Button>("PassiveIncomeButton");
		if (texture.Visible == false)
		{
			texture.Visible = true;
			EmitSignal(SignalName.CloseOtherDropdowns);
		}
		else
		{
			OnCloseDropdown();

		}
	}
	private void OnCloseDropdown()
	{
		var texture = GetNode<TextureRect>("CatNipExchangeTexture");
		texture.Visible = false;
	}

}
