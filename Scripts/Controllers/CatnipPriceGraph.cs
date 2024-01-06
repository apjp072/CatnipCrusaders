using Godot;
using System.Collections.Generic;

public partial class CatnipPriceGraph : Line2D
{
    public int GraphWidth = 750;
    public int GraphHeight = 375;
    public Vector2 Offset = new Vector2(40, 600);
    public int maxPrice = 100;

    public override void _Ready()
    {
        Width = 7; //line width
    }

    public void CreateGraph(List<float> prices)
    {
        if (prices == null || prices.Count == 0)
            return;

        ClearPoints(); // Clear existing points

        // Determine the starting index for the graph
        int startIdx = prices.Count > 10 ? prices.Count - 10 : 0;
        List<float> lastPrices = prices.GetRange(startIdx, prices.Count - startIdx);

        int minPrice = 0; // Minimum price (assumed to be 0)
        float priceRange = maxPrice - minPrice; // Price range for scaling
        float scaleX = GraphWidth / (float)(lastPrices.Count - 1); // X-axis scaling factor
        float scaleY = priceRange > 0 ? GraphHeight / priceRange : 1; // Y-axis scaling factor

        for (int i = 0; i < lastPrices.Count; i++)
        {
            // Calculate the X and Y coordinates for each data point
            float x = Offset.X + (i * scaleX);
            float y = Offset.Y + (GraphHeight - ((lastPrices[i] - minPrice) * scaleY)) - GraphHeight;
            AddPoint(new Vector2(x, y)); // Add the data point to the graph
        }
    }
}
