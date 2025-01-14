using Godot;
using System;

public partial class InteractionHandler : Node
{
	private RayCast2D interactionCast;
	private TileMapLayer doorsTileMapLayer;

	private string interactableName;
	private Vector2I raycastCorrectionVector;

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

			//FIXME: This is a temporary solution, should be fixed... who am I lying to, this is a mess
			if (interactionCast.RotationDegrees == 180) {
				raycastCorrectionVector = new Vector2I(-1, 0);
			} else if (interactionCast.RotationDegrees == 90) {
				raycastCorrectionVector = new Vector2I(0, 1);
			} else {
				raycastCorrectionVector = new Vector2I(0, 0);
			}

			switch (interactableName) {
				case "Doors":
					Vector2 collisionPoint = interactionCast.GetCollisionPoint();
					Vector2 tilePosition = doorsTileMapLayer.LocalToMap(collisionPoint);
					Vector2I tilePositionInt = new Vector2I((int)tilePosition.X, (int)tilePosition.Y);
					tilePositionInt += raycastCorrectionVector;
					SwitchFenceDoor(tilePositionInt);
					break;
				default:
					break;
			}
		}
	}

	//TODO: Move door logic to a separate script
	private void SwitchFenceDoor(Vector2I tilePosition) {
		TileData tileData = doorsTileMapLayer.GetCellTileData(tilePosition);
		bool isOpen = tileData.GetCustomData("isOpen").AsBool();
		if (isOpen) {
			CloseFenceDoor(tilePosition);
		} else {
			OpenFenceDoor(tilePosition);
		}
	}

	private void OpenFenceDoor(Vector2I tilePosition) {
		TileData tileData = doorsTileMapLayer.GetCellTileData(tilePosition);
		bool isFenceDoorMiddle = tileData.GetCustomData("FenceDoorMiddle").AsBool();
		if (isFenceDoorMiddle) {
			doorsTileMapLayer.SetCell(tilePosition, 1, new Vector2I(4,2));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1,0), 1, new Vector2I(5,2));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1,0), 1, new Vector2I(3,2));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1,-1), 1, new Vector2I(3,1));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1,-1), 1, new Vector2I(5,1));
		}
	}

	private void CloseFenceDoor(Vector2I tilePosition) {
		TileData tileData = doorsTileMapLayer.GetCellTileData(tilePosition);
		bool isFenceDoorMiddle = tileData.GetCustomData("FenceDoorMiddle").AsBool();
		if (isFenceDoorMiddle) {
			doorsTileMapLayer.SetCell(tilePosition, 1, new Vector2I(4,0));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1,0), 1, new Vector2I(5,0));
			doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1,0), 1, new Vector2I(3,0));
			doorsTileMapLayer.EraseCell(tilePosition + new Vector2I(-1,-1));
			doorsTileMapLayer.EraseCell(tilePosition + new Vector2I(1,-1));
		}
	}
}
