using Godot;
using System;

public partial class Player : CharacterBody2D
{
    // Signal emitted when the player is hit
    [Signal]
    public delegate void HitEventHandler();

    // Player attributes and properties
    public Vector2 ScreenSize; // size of the game window
    public float speedMultiplier = 1;
    public Vector2 velocity;

    [Export]
    public int max_speed = 1000;
    [Export]
    public float gravity = 85;
    [Export]
    public int jump_force = 2000;
    [Export]
    public int acceleration = 50;
    [Export]
    public int jumpBufferTime = 8; // frames where you can press jump before you hit the ground
    [Export]
    public int fallBufferTime = 8;

    public bool isMoving = false;
    public int initialMovingBuffer = 60;
    public int isMovingBuffer;

    public int framesMoving = 0;
    public int totalFrames = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
        isMovingBuffer = initialMovingBuffer;
    }

    private int jumpBufferCounter = 0;
    private int fallBufferCounter = 0;

    // Called in every physics process frame
    public override void _PhysicsProcess(double delta)
    {
        var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        // Check if player is on the floor
        if (IsOnFloor())
        {
            fallBufferCounter = fallBufferTime;
            if (Velocity.X == 0 && Velocity.Y == 0)
            {
                // Player is idle on the floor
            }
        }

        // Check if player is in the air, if so, "enable" gravity
        if (!IsOnFloor())
        {
            if (fallBufferCounter > 0) fallBufferCounter -= 1;

            velocity.Y += gravity;
            if (velocity.Y > 2000) velocity.Y = 2000;
        }

        // Check for right movement input
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += acceleration;
            animatedSprite.FlipH = false;
            isMovingBuffer = initialMovingBuffer;
        }
        // Check for left movement input
        else if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= acceleration;
            animatedSprite.FlipH = true;
            isMovingBuffer = initialMovingBuffer;
        }
        else
        {
            // Apply gradual deceleration if no movement input
            velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.2);
        }

        // Limit horizontal velocity
        velocity.X = Math.Clamp(velocity.X, -max_speed, max_speed);

        // Check for jump input
        if (Input.IsActionPressed("move_up"))
        {
            jumpBufferCounter = jumpBufferTime;
            isMovingBuffer = initialMovingBuffer;
        }

        // Handle jumping with buffer
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= 1;
        }
        if (jumpBufferCounter > 0 && fallBufferCounter > 0)
        {
            velocity.Y = -jump_force;
            jumpBufferCounter = 0;
            fallBufferCounter = 0;
        }

        // Handle releasing jump input
        if (Input.IsActionJustReleased("move_up"))
        {
            if (velocity.Y < 0)
            {
                velocity.Y += 400;
            }
        }

        // Apply the calculated velocity and perform collision detection
        Velocity = velocity;
        MoveAndSlide();

        // Update player's animation based on movement
        if (Math.Abs(Velocity.X) > 0)
        {
            animatedSprite.Animation = "right";
        }
        if (Math.Abs(Velocity.Y) > 0)
        {
            animatedSprite.Animation = "up";
        }
        if (Velocity.Length() > 0)
        {
            // Player is moving
        }
        else
        {
            // Player is idle
            animatedSprite.Animation = "idle";
            animatedSprite.Play();
        }

        // Keep player within screen boundaries
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        // Check if the player is considered moving
        if (isMovingBuffer > 0)
        {
            isMovingBuffer--;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Update frame counters to track player movement
        if (Velocity.X != 0 || Velocity.Y != 0)
        {
            framesMoving += 1;
        }
        totalFrames += 1;
    }

    // Reset frame counters
    public void resetFrames()
    {
        framesMoving = 0;
        totalFrames = 0;
    }

    // Calculate the percentage of frames with movement
    public float getFrameTotal()
    {
        return (float)framesMoving / (float)totalFrames;
    }

    // Method to start the player at a specific position
    public void Start(Vector2 position)
    {
        Position = position;
        Show();
    }
}
