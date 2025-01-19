using Godot;
using System.Collections.Generic;

public partial class PlayerCharacter : CharacterBody2D {
	private float base_speed = 70;
	private const float RUNNING_MULTIPLIER = 1.8f;
	private float speed;
	private bool isMoving = false;
	private bool isRunning = false;
	private int stamina = 100;
	private bool hasStamina = true;

	private TileMapLayer elevationTileMapLayer;
	private bool isAboveGround = false;

	private AnimatedSprite2D animatedSprite;
	private Vector2 lastDirection = Vector2.Down;

	private RayCast2D interactionCast;

	private readonly Dictionary<Vector2, string> animationNames = new Dictionary<Vector2, string> {
		{ Vector2.Up, "up" },
		{ Vector2.Down, "down" },
		{ Vector2.Left, "side" },
		{ Vector2.Right, "side" }
	};

	public override void _Ready() {
		base_speed = this.GetMeta("speed").AsInt32();
		stamina = this.GetMeta("stamina").AsInt32();

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		elevationTileMapLayer = GetNode<TileMapLayer>("../Level/Elevations");
		interactionCast = GetNode<RayCast2D>("InteractionCast");
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 direction = Input.GetVector("walking_left", "walking_right", "walking_up", "walking_down");

		isAboveGround = CheckIfElevated();
		hasStamina = stamina > 0;
		isMoving = direction != Vector2.Zero;

		if (isAboveGround) {
			this.SetCollisionMaskValue(1, false);
		}  else {
			this.SetCollisionMaskValue(1, true);
		}

		isRunning = Input.IsActionPressed("running");
		if (isRunning && hasStamina) {
			speed = base_speed * RUNNING_MULTIPLIER;
		} else {
			speed = base_speed;
		}

		UpdateStamina();

		if (direction != Vector2.Zero) {
			lastDirection = direction;

			if (direction.X < 0) {
				animatedSprite.FlipH = true;
			} else if (direction.X >= 0) {
				animatedSprite.FlipH = false;
			}

			if (isRunning && hasStamina) {
				PlayRunningAnimation(direction);
			} else {
				PlayWalkingAnimation(direction);
			}
			Velocity = direction.Normalized() * speed;

			UpdateInteractionCastRotation(direction);
		} else {
			Velocity = Vector2.Zero;
			PlayIdleAnimation();
		}

		MoveAndSlide();
	}

	private void UpdateStamina() {
		if (isRunning && hasStamina && isMoving) {
			DecreaseStamina();
		} else if (!isRunning) {
			IncreaseStamina();
		}
	}

	private void DecreaseStamina() {
		if (stamina > 0) {
			stamina -= 1;
			this.SetMeta("stamina", stamina);
		}
	}

	private void IncreaseStamina() {
		if (stamina < 100) {
			stamina += 1;
			this.SetMeta("stamina", stamina);
		}
	}

	private void UpdateInteractionCastRotation(Vector2 direction)
	{
		// Ugly solution to prioritize the side movement, but it works
		// Otherwise the interactionCast would be rotated to the direction
		// that was first pressed
		if (direction.X < 0) {
			interactionCast.RotationDegrees = 180;
		} else if (direction.X > 0) {
			interactionCast.RotationDegrees = 0;
		} else if (direction == Vector2.Up) {
			interactionCast.RotationDegrees = -90;
		} else if (direction == Vector2.Down) {
			interactionCast.RotationDegrees = 90;
		}
	}

	private bool CheckIfElevated() {
		Vector2 playerPosition = Position;
		Vector2I cell = elevationTileMapLayer.LocalToMap(playerPosition);

		var cellTileData = elevationTileMapLayer.GetCellTileData(cell);

		if (cellTileData != null){
			if (cellTileData.GetCustomData("elevated").AsString() == "true") {
				return true;
			}
		}

		return false;
	}

	private void PlayIdleAnimation() {
		if (animationNames.ContainsKey(lastDirection)) {
			string idleAnimation = animationNames[lastDirection] + "_idle";
			animatedSprite.Play(idleAnimation);
		} else {
			animatedSprite.Play("side_idle");
		}
	}

	private void PlayWalkingAnimation(Vector2 direction) {
		if (animationNames.ContainsKey(direction)) {
			animatedSprite.Play(animationNames[direction]);
		} else {
			animatedSprite.Play("side");
		}
	}

	private void PlayRunningAnimation(Vector2 direction) {
		if (animationNames.ContainsKey(direction)) {
			animatedSprite.Play(animationNames[direction] + "_running");
		} else {
			animatedSprite.Play("side_running");
		}
	}
}
