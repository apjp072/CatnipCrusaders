using Godot;
using System;

public partial class PassiveIncomeDropDown : Control
{
    // Signal emitted when other dropdowns should be closed
    [Signal]
    public delegate void CloseOtherDropdownsEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var texture = GetNode<TextureRect>("TextureRect");

        // Initialize the dropdown as hidden
        Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Additional processing logic can be added here if needed
    }

    // Method called when the button associated with this dropdown is pressed
    public void OnButtonPressed()
    {
        var texture = GetNode<TextureRect>("TextureRect");

        // Toggle visibility of the dropdown and emit a signal to close other dropdowns
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

    // Method to close the dropdown
    public void OnCloseDropdown()
    {
        var texture = GetNode<TextureRect>("TextureRect");

        // Hide the dropdown when closing it
        texture.Visible = false;
    }
}
