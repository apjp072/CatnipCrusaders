using Godot;
using System;
using Godot.Collections;
using System.IO;

public partial class ShopsController : Control
{
    // References to dropdown menus
    ItemShopDropDown itemShopDropDown;
    PassiveIncomeDropDown passiveIncomeDropDown;
    CatnipExchange catnipExchangeDropDown;

    // Variables to manage shop positions and states
    private Vector2 originalPosition;
    private bool isMoved = false;
    private Label toggleShopsLabel;

    // Variables to track purchased items
    private bool boughtMouseTrap = false;
    private bool boughtCatnip = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialize references to dropdown menus
        itemShopDropDown = GetNode<ItemShopDropDown>("ItemShopDropDown");
        passiveIncomeDropDown = GetNode<PassiveIncomeDropDown>("PassiveIncomeDropDown");
        catnipExchangeDropDown = GetNode<CatnipExchange>("CatnipExchange");
        OnOpenItemShop();

        // Store the original position and set the initial label text
        originalPosition = Position;
        toggleShopsLabel = GetNode<Label>("ToggleShopsLabel");
        toggleShopsLabel.Text = "OPeN";

    }

    // Open the item shop dropdown
    private void OnOpenItemShop()
    {
        GD.Print("Opening item shop.");
        itemShopDropDown.Visible = true;
        OnClosePassiveIncome();
        OnCloseCatnipExchange();
    }

    // Close the item shop dropdown
    public void OnCloseItemShop()
    {
        GD.Print("Closing item shop.");
        itemShopDropDown.Visible = false;
    }

    // Open the passive income shop dropdown (if the mouse trap is bought)
    private void OnOpenPassiveIncome()
    {
        if (boughtMouseTrap)
        {
            GD.Print("Opening Passive Income shop.");
            passiveIncomeDropDown.Visible = true;
            OnCloseItemShop();
            OnCloseCatnipExchange();
        }
    }

    // Close the passive income shop dropdown
    public void OnClosePassiveIncome()
    {
        GD.Print("Closing Passive Income shop.");
        passiveIncomeDropDown.Visible = false;
    }

    // Open the catnip exchange shop dropdown (if both the mouse trap and catnip are bought)
    private void OnOpenCatnipExchange()
    {
        if (boughtMouseTrap && boughtCatnip)
        {
            GD.Print("Opening Catnip Exchange shop.");
            catnipExchangeDropDown.Visible = true;
            OnCloseItemShop();
            OnClosePassiveIncome();
        }
    }

    // Close the catnip exchange shop dropdown
    public void OnCloseCatnipExchange()
    {
        GD.Print("Closing Catnip Exchange shop.");
        catnipExchangeDropDown.Visible = false;
    }

    // Handle button press to toggle shop positions
    private void OnToggleShopsButtonPress()
    {
        Tween tween = GetTree().CreateTween();

        Vector2 targetPosition;
        if (isMoved)
        {
            // Move back to the original position
            targetPosition = originalPosition;
            toggleShopsLabel.Text = "OPeN";
        }
        else
        {
            // Move 500px to the right
            targetPosition = originalPosition + new Vector2(900, 0);
            toggleShopsLabel.Text = "CLOSe";
        }

        // Create a tween to animate the 'rect_position' property
        tween.TweenProperty(this, "position", targetPosition, 0.2f);

        // Toggle the moved state
        isMoved = !isMoved;
    }

    // Handle the event when the mouse trap item is bought
    public void OnBoughtMouseTrap()
    {
        boughtMouseTrap = true;
        var passiveIncomeClassified = GetNode<TextureRect>("PassiveIncomeClassified");
        passiveIncomeClassified.Visible = false;
        OnOpenPassiveIncome();

    }

    // Handle the event when the catnip item is bought
    public void OnBoughtCatnip()
    {
        boughtCatnip = true;
        var catnipClassified = GetNode<TextureRect>("CatnipClassified");
        catnipClassified.Visible = false;
        OnOpenCatnipExchange();

    }

    // Handle the event to save the game progress
    public void OnSaveGame()
    {
        itemShopDropDown.SaveGame();

    }
}
