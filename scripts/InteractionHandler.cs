using Godot;
using System;

public partial class InteractionHandler : Node
{
	private RayCast2D interactionCast;

    public override void _Ready() {
		interactionCast = GetNode<RayCast2D>("../InteractionCast");
	}

	public override void _Process(double delta){
		if (Input.IsActionJustPressed("interaction")) {
			CheckForInteractable();
		}
	}

	private void CheckForInteractable() {
		if (interactionCast.IsColliding()) {
			GD.Print("Interactable");
		}
	}
}
