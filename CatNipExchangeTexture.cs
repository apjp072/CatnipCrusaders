using Godot;
using System;
using System.Collections.Generic; // Include this for using List

public partial class CatNipExchangeTexture : TextureRect
{
    // Imported Variables
    private main mainNode;
    private Player player;
    private int score;

    // Local Variables 
    private int catnipCost = 20;
    private int catnipQuantity = 0;

    private int totalProfit = 0;
    private int totalValue = 0;
    private double catnipAging = 1.2; // Reset on Line 91
    [Export]
    private double catnipAgingMultiplier = 0.1;
    private int totalProfitAging;
    private List<float> catnipPriceHistory = new List<float>();

    // Local Nodes
    private Label catnipQuantityLabel;
    private Label catnipCostLabel;
    private Label marketVolatilityLabel;
    private Label totalValueLabel;
    private Label totalProfitLabel;
    private Timer updateCatnipCostTimer;
    private CatnipPriceGraph catnipPriceGraph;

    // Market Volatility
    private TextureRect marketVolatilityTexture;
    private TextureRect marketVolatilityColor;
    private Timer marketVolatilityTimer;
    private float marketVolatility = 1;
    private bool updateMarketVolatility = true;

    public override void _Ready()
    {
        // Get references to other nodes and initialize variables
        mainNode = GetTree().Root.GetNode<main>("Main");
        score = mainNode.GetScore();
        player = mainNode.GetNode<Player>("Player");

        catnipQuantityLabel = GetNode<Label>("CatnipQuantityLabel");
        catnipCostLabel = GetNode<Label>("CatnipCostLabel");
        marketVolatilityLabel = GetNode<Label>("MarketVolatilityLabel");
        totalValueLabel = GetNode<Label>("TotalValueLabel");
        totalProfitLabel = GetNode<Label>("TotalProfitLabel");
        updateCatnipCostTimer = GetNode<Timer>("UpdateCatnipCostTimer");
        catnipPriceGraph = GetNode<TextureRect>("CatnipPriceGraphTexture").GetNode<CatnipPriceGraph>("CatnipPriceGraph");

        marketVolatilityTexture = GetNode<TextureRect>("MarketVolatilityTexture");
        marketVolatilityColor = marketVolatilityTexture.GetNode<TextureRect>("MarketVolatilityColor");
        marketVolatilityTimer = marketVolatilityTexture.GetNode<Timer>("MarketVolatilityTimer");
        marketVolatilityTimer.WaitTime = updateCatnipCostTimer.WaitTime / 2;

        catnipPriceHistory.Add(catnipCost); // Adding the initial value to the price history

        updateCatnipCostTimer.Start();
        marketVolatilityTimer.Start();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Any per-frame processing can go here if needed
    }

    public void UpdateCatnipCost()
    {
        // Calculate the new catnip cost based on market volatility
        catnipCost = ((int)(GD.Randi() % 50) * (100 - (int)marketVolatility + 1)) % 100;

        if (catnipCost < 1)
        {
            catnipCost = 1;
        }
        if (catnipCost > 100)
        {
            catnipCost = 100;
        }

        // Update UI
        catnipCostLabel.Text = "$" + FormatNumber(catnipCost);

        // Calculate total profit
        totalValue = catnipCost * catnipQuantity;

        // Update the catnip price history graph
        catnipPriceHistory.Add(catnipCost);
        catnipPriceGraph.CreateGraph(catnipPriceHistory);

        GD.Print("Catnip Aging Value: " + catnipAging);

        // Calculate total profit with aging
        if (catnipQuantity != 0)
        {
            if (catnipAging <= 1)
            {
                totalProfitLabel.Text = "$" + FormatNumber((int)(totalValue * catnipAging));
            }
            else
            {
                totalProfitLabel.Text = "$" + FormatNumber(totalValue);
            }

            if (catnipAging <= 0.1)
            {
                OnSellAllButtonPress();
                catnipAging = 1.2;
            }
            else
            {
                catnipAging -= catnipAgingMultiplier;
                GD.Print("Catnip Age = " + catnipAging);
            }
        }
        else
        {
            catnipAging = 1.2;
            totalProfitLabel.Text = "$" + FormatNumber(totalValue);
        }
    }

    public void OnMarketVolatilityTimeout()
    {
        GD.Print("UpdateMarketVolatility: " + updateMarketVolatility);
        if (updateMarketVolatility == true)
        {
            marketVolatility = (1 - mainNode.GetNode<Player>("Player").getFrameTotal()) * 100;

            float hue = (100 - marketVolatility) / 365;

            Color volatilityColorValue = Color.FromHsv(hue, 1, 1);
            marketVolatilityColor.Modulate = volatilityColorValue;
            GD.Print("Market volatility:" + marketVolatility + ", Color value: " + volatilityColorValue);
            updateMarketVolatility = !updateMarketVolatility;

            mainNode.GetNode<Player>("Player").resetFrames();
        }
        else
        {
            updateMarketVolatility = !updateMarketVolatility;
        }
    }

    private void OnBuyOneButtonPress()
    {
        score = mainNode.GetScore();

        if (score >= catnipCost)
        {
            mainNode.IncrementScoreBy(-1 * catnipCost); // Increment score

            catnipQuantity++;

            catnipQuantityLabel.Text = FormatNumber(catnipQuantity);

            totalValue += catnipCost;

            totalValueLabel.Text = "$" + FormatNumber(totalValue);
        }
    }

    private void OnBuyAllButtonPress()
    {
        score = mainNode.GetScore();

        if (score >= catnipCost)
        {
            int maxCatnipPurchasable = (int)Math.Floor((double)score / catnipCost);

            mainNode.IncrementScoreBy(-1 * maxCatnipPurchasable * catnipCost); // Increment score

            catnipQuantity += maxCatnipPurchasable;

            catnipQuantityLabel.Text = FormatNumber(catnipQuantity);

            totalValue += catnipCost * maxCatnipPurchasable;

            totalValueLabel.Text = "$" + FormatNumber(totalValue);
        }
    }

    private void OnSellOneButtonPress()
    {
        if (catnipQuantity > 0)
        {
            catnipQuantity--;
            catnipQuantityLabel.Text = FormatNumber(catnipQuantity);

            totalValue -= catnipCost;
            totalValueLabel.Text = "$" + FormatNumber(totalValue);

            if (catnipAging > 1)
            {
                mainNode.IncrementScoreBy(catnipCost);
                totalProfitLabel.Text = "$" + totalProfit;
            }
            else
            {
                mainNode.IncrementScoreBy((int)catnipAging * catnipCost);
                totalProfitLabel.Text = "$" + totalProfit;
            }
        }
    }

    private void OnSellAllButtonPress()
    {
        if (catnipQuantity > 0)
        {
            if (catnipAging > 1)
            {
                mainNode.IncrementScoreBy(catnipCost * catnipQuantity);
            }
            else
            {
                mainNode.IncrementScoreBy((int)catnipAging * catnipCost * catnipQuantity);
            }

            catnipQuantity = 0;

            catnipQuantityLabel.Text = "0"; //reset values

            totalValue = 0;
            totalValueLabel.Text = "$" + "0";

            totalProfit = 0;
            totalProfitLabel.Text = "$" + "0";
        }
    }

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
        else if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0") + "M";
        }
        return number.ToString();
    }
}
