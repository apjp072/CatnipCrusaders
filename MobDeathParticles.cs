using Godot;
using System;

public partial class MobDeathParticles : CpuParticles2D
{
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Emitting = true; // Start emitting particles

        // Assuming the Timer node is named "LifetimeTimer"
        var timer = GetNode<Timer>("Timer");
        timer.Start();
    }

    private void OnLifetimeTimerTimeout()
    {
        QueueFree(); // Remove the particle effect from the scene
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
