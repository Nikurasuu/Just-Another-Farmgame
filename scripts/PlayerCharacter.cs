using Godot;
using System.Collections.Generic;

public partial class PlayerCharacter : CharacterBody2D {
    private const float BASE_SPEED = 60;
    private const float RUNNING_MULTIPLIER = 1.8f;

    private float speed = BASE_SPEED;
    private bool isRunning = false;

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
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        elevationTileMapLayer = GetNode<TileMapLayer>("../Level/Elevations");
        interactionCast = GetNode<RayCast2D>("InteractionCast");
    }

    public override void _PhysicsProcess(double delta) {
        Vector2 direction = Input.GetVector("walking_left", "walking_right", "walking_up", "walking_down");

        isAboveGround = CheckIfElevated();

        if (isAboveGround) {
            this.SetCollisionMaskValue(1, false);
        }  else {
            this.SetCollisionMaskValue(1, true);
        }

        isRunning = Input.IsActionPressed("running");
        if (isRunning) {
            speed = BASE_SPEED * RUNNING_MULTIPLIER;
        } else {
            speed = BASE_SPEED;
        }

        if (direction != Vector2.Zero) {
            lastDirection = direction;

            if (direction.X < 0) {
                animatedSprite.FlipH = true;
            } else if (direction.X >= 0) {
                animatedSprite.FlipH = false;
            }

            if (isRunning) {
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