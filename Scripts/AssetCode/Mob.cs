using Godot;
using System;

public partial class Mob : RigidBody2D
{
    [Signal]
    public delegate void MobDiedEventHandler();

    // Variables for mob movement and behavior
    private float moveSpeed = 1000.0f;  // Speed of the mob movement
    private float jumpForce = 700.0f;   // Force applied for jumping
    private float maxSpeed = 1400.0f;   // Maximum horizontal speed
    private float minSpeed = 600.0f;    // Minimum horizontal speed
    private int changeDirectionCooldown = 0;  // Cooldown before changing direction
    public float speedMultiplier = 1;   // Multiplier for speed adjustments
    private int maxCooldown = 20;       // Maximum cooldown value

    private CollisionShape2D collisionShape2DNode;

    // References to other nodes
    private Marker2D leftSpawnPoint;
    private Marker2D rightSpawnPoint;
    private Player playerNode;
    private main mainNode;

    private bool onFloor = false;   // Flag indicating if the mob is on the floor
    private CollisionObject2D floor;  // Reference to the floor object
    private CharacterBody2D player;   // Reference to the player object

    private Vector2 lastPosition;   // Stores the last position of the mob
    private int frameCounter = 0;   // Counter for frames
    private const int frameThreshold = 150;  // Threshold for frame counting

    public override void _Ready()
    {
        // Initialize mob behavior
        AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);

        // Connect the mob to the "mobs" group for easier management
        AddToGroup("mobs");

        // Get references to other important nodes
        mainNode = GetTree().Root.GetNode<main>("Main");
        leftSpawnPoint = mainNode.GetNode<Marker2D>("LeftSpawnPoint");
        rightSpawnPoint = mainNode.GetNode<Marker2D>("RightSpawnPoint");
        playerNode = mainNode.GetNode<Player>("Player");

        collisionShape2DNode = GetNode<CollisionShape2D>("CollisionShape2D");

        lastPosition = Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        RandomMovement();
        CheckAndMoveToCenter();
    }

    // Method for random mob movement behavior
    private void RandomMovement()
    {
        if (GD.Randi() % 5 == 0)  // Randomly decides to move
        {
            if (changeDirectionCooldown <= 0 && onFloor == true)
            {
                int action = (int)(GD.Randi() % 2);
                switch (action)
                {
                    case 0: MoveLeft(); break;
                    case 1: MoveRight(); break;
                    case 2: MoveToCenter(); break;
                }
                changeDirectionCooldown = (int)(GD.Randf() * maxCooldown); // Random cooldown
            }
            else
            {
                changeDirectionCooldown--;
            }
        }
    }

    // Method to move the mob to the left
    private void MoveLeft()
    {
        CallDeferred(nameof(MoveLeftDeferred));  // Engine process
    }

    // Deferred method for moving the mob to the left
    private void MoveLeftDeferred()
    {
        float randomSpeed = GD.Randf() * (maxSpeed - minSpeed) + minSpeed;
        randomSpeed *= speedMultiplier;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
        LinearVelocity = new Vector2(-randomSpeed, LinearVelocity.Y);
        LinearVelocity *= speedMultiplier;
        Jump();
    }

    // Method to move the mob to the right
    private void MoveRight()
    {
        CallDeferred(nameof(MoveRightDeferred));  // Engine process
    }

    // Deferred method for moving the mob to the right
    private void MoveRightDeferred()
    {
        float randomSpeed = GD.Randf() * (maxSpeed - minSpeed) + minSpeed;
        randomSpeed *= speedMultiplier;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
        LinearVelocity = new Vector2(randomSpeed, LinearVelocity.Y);
        LinearVelocity *= speedMultiplier;
        Jump();
    }

    // Method for mob to jump
    private void Jump()
    {
        // Check if the mob is on the floor and can jump
        float dTLSP = Position.X - leftSpawnPoint.Position.X;
        float dTRSP = rightSpawnPoint.Position.X - Position.X;
        bool farFromWall = dTLSP > 50 && dTRSP > 50;

        if (onFloor && Math.Abs(LinearVelocity.Y) < 0.01 && farFromWall)
        {
            int action = (int)(GD.Randi() % 5);
            if (action == 0)
            {
                LinearVelocity = new Vector2(LinearVelocity.X, -jumpForce * (GD.Randf() + 0.5f));
                onFloor = false;
            }
        }
        else if (!farFromWall)
        {
            int action = (int)(GD.Randi() % 2);
            if (action == 0)
            {
                MoveToCenter();
            }
        }
    }

    // Method to move the mob toward the center
    private void MoveToCenter()
    {
        float distanceToLeftSpawnPoint = Position.X - leftSpawnPoint.Position.X;
        float distanceToRightSpawnPoint = rightSpawnPoint.Position.X - Position.X;

        int goAwayFromWallChance = (int)(GD.Randi() % 100000);
        if (distanceToRightSpawnPoint < distanceToLeftSpawnPoint && goAwayFromWallChance != 0)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

	private void MoveAwayFromPlayer()
	{
		Vector2 playerPosition = playerNode.Position;
		if (playerPosition.X > Position.X)
		{
			MoveLeft();
		}
		else MoveRight();
	}
	private void Wait()
	{
		//do nothing
	}

	private void CheckAndMoveToCenter()
	{
		frameCounter++;

		if (frameCounter >= frameThreshold)
		{
			if (Position.DistanceTo(lastPosition) < 0.1) // Adjust the threshold as needed
			{
				ForceMoveToCenter();
			}

			// Reset frame counter and update last position
			frameCounter = 0;
			lastPosition = Position;
		}
	}

	private void ForceMoveToCenter()
	{
		float distanceToLeftSpawnPoint = Position.X - leftSpawnPoint.Position.X;
		float distanceToRightSpawnPoint = rightSpawnPoint.Position.X - Position.X;

		if (distanceToRightSpawnPoint < distanceToLeftSpawnPoint)
		{
			Position = new Vector2(Position.X-10, Position.Y + 2); // Maintain the current Y position
		}
		else
		{
			Position = new Vector2(Position.X+10, Position.Y + 2); // Maintain the current Y position
		}
		
	}



	public void Die()
	{
		TriggerParticles();
		QueueFree();
		EmitSignal("MobDied");

	}
	public void ChangeColor(Color mobColor)
	{
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Modulate = new Color(mobColor);
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();

	}

	public void SetFloor(CollisionObject2D floorNode)
	{
		floor = floorNode;
	}
	public void SetPlayer(CharacterBody2D Player)
	{
		player = Player;
	}
	private void OnBodyEntered(Node body)
	{
		if (body == floor)
		{
			GD.Print("TOUCHIN FLOOR!");
			onFloor = true;
		}

		else if (body == player && playerNode.isMoving == true) 
		{
			GD.Print("MaxMobs:" + mainNode.MaxMobs + ", mobsCollected: " + mainNode.GetMobsCollected());
			if (mainNode.MaxMobs > mainNode.GetMobsCollected())
			{
				mainNode.IncreaseMobsCollected();
				Die();

			}
			else
			{
				MoveAwayFromPlayer();
			}

		}
	}

	private void TriggerParticles()
	{
		PackedScene particleScene = (PackedScene)ResourceLoader.Load("res://mob_death_particles.tscn");
		Node2D particles = (Node2D)particleScene.Instantiate();
		GetParent().AddChild(particles);
		particles.GlobalPosition = GlobalPosition;
	}
	private void makeCollisionFalse()
	{
		MoveAwayFromPlayer();
	}
	private void makeCollisionTrue()
	{
		Die();
	}



}
