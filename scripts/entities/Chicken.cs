using Godot;
using System;

public partial class Chicken : CharacterBody2D {
    private const float BASE_SPEED = 40;

    // lazyness is the percent chance of not moving
    private int lazyness = 100;

    private bool isMoving = false;
    private Vector2 direction = Vector2.Zero;

    private AnimatedSprite2D animatedSprite;
    
    private Timer movingTimer;
    private static Random random = new Random();

    [Signal]
    public delegate void PetEventHandler();

    public override void _Ready() {
        lazyness = this.GetMeta("lazyness").AsInt32();

        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        movingTimer = GetNode<Timer>("MovingTimer");
        movingTimer.Timeout += OnMovingTimerTimeout;

        Pet += GetPet;
    }

    public override void _PhysicsProcess(double delta) {
        if (isMoving) {
            PlayWalkingAnimation(direction);
            Velocity = direction.Normalized() * BASE_SPEED;
        } else {
            Velocity = Vector2.Zero;
            PlayIdleAnimation();
        }

        MoveAndSlide();
    }

    private void OnMovingTimerTimeout() {
        MovingEvent();
    }

    private void MovingEvent() {
        if (isMoving) {
            isMoving = false;
            movingTimer.Start(10);
        } else {
            if (random.Next(0, 100) >= lazyness) { 
                direction = DecideNewDirection();
                isMoving = true;
                movingTimer.Start(0.5 + random.NextDouble() * 1);
            } else {
                movingTimer.Start(10);
            }
        }
    }

    private Vector2 DecideNewDirection() {
        float x = (float)(random.NextDouble() * 2 - 1);
        float y = (float)(random.NextDouble() * 2 - 1);
        return new Vector2(x, y);
    }

    private void PlayIdleAnimation() {
        animatedSprite.Play("idle");
    }

    private void PlayWalkingAnimation(Vector2 direction) {
        if (direction.Y <= 0) {
            animatedSprite.Play("walking_up");
            if (direction.X >= 0) {
                animatedSprite.FlipH = true;
            } else {
                animatedSprite.FlipH = false;
            }
        } else {
            animatedSprite.Play("walking_down");
            if (direction.X >= 0) {
                animatedSprite.FlipH = true;
            } else {
                animatedSprite.FlipH = false;
            }
        }
    }

    private void GetPet() {
        if(this.GetMeta("alreadyPetted").AsBool()) {
            GD.Print("Chicken already petted");
        } else {
            this.SetMeta("alreadyPetted", true);
            GD.Print("Chicken petted");
        }
    }
}