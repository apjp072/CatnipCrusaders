using Godot;
using System;

public partial class LightSwitch : Control
{
    private float flickerIntervalMin = 0.1f; // Minimum time between flickers
    private float flickerIntervalMax = 0.5f; // Maximum time between flickers

    private TextureRect lightOn;
    private TextureRect lightOff;
    private Light2D pointLight;

    private bool isLightOn = false;

    public override void _Ready()
    {
        lightOn = GetNode<TextureRect>("LightOn");
        lightOff = GetNode<TextureRect>("LightOff");
        pointLight = GetNode<Light2D>("PointLight2D");

        // Start the flickering process
        FlickerLight();
    }

    private async void FlickerLight()
    {
        while (true)
        {
            isLightOn = !isLightOn;

            // Update the visibility based on the light state
            lightOn.Visible = isLightOn;
            lightOff.Visible = !isLightOn;
            pointLight.Visible = isLightOn;

            // Wait for a random interval before the next flicker
            float nextFlicker = (float)GD.RandRange(flickerIntervalMin, flickerIntervalMax);
            await ToSignal(GetTree().CreateTimer(nextFlicker), "timeout");
        }
    }
}
