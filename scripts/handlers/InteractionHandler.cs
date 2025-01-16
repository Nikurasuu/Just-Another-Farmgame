using Godot;

public partial class InteractionHandler : Node
{
	private RayCast2D interactionCast;
	private TileMapLayer doorsTileMapLayer;

	private string interactableName;
	private Vector2 raycastCorrectionVector;

	[Signal]
	public delegate void PetEventHandler();

    public override void _Ready() {
		interactionCast = GetNode<RayCast2D>("../InteractionCast");

		// TODO: Add better error handling, for levels without doors
		doorsTileMapLayer = GetNode<TileMapLayer>("../../Level/Doors");

	}

	public override void _Process(double delta){
		if (Input.IsActionJustPressed("interaction")) {
			CheckForInteractable();
		}
	}

	private void CheckForInteractable() {

		if (interactionCast.IsColliding()) {

			Node2D interactable = interactionCast.GetCollider() as Node2D;
			interactableName = interactable.Name;

			Vector2 collisionPoint = interactionCast.GetCollisionPoint();

			//FIXME: This is a temporary solution, should be fixed... who am I lying to, this is a mess
			if (interactionCast.RotationDegrees == 180) {
				raycastCorrectionVector = new Vector2(-16, 0);
			} else if (interactionCast.RotationDegrees == 90) {
				raycastCorrectionVector = new Vector2(0, 16);
			} else {
				raycastCorrectionVector = new Vector2(0, 0);
			}
			collisionPoint += raycastCorrectionVector;

			switch (interactableName) {
				case "Doors":
					FenceDoorHandler.SwitchFenceDoor(doorsTileMapLayer, collisionPoint);
					break;
				default:
					//Read metadata of interactable and check if petable, if so, pet the interactable
					if (interactable.GetMeta("petable").AsBool()) {
						interactable.EmitSignal(SignalName.Pet);
					}

					break;
			}
		}
	}
}
