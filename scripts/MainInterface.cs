using Godot;
using System;

public partial class MainInterface : Control {

	private CharacterBody2D player;
	private TextureProgressBar staminaBar;

	public override void _Ready() {
		staminaBar = GetNode<TextureProgressBar>("StaminaBar");
		player = GetNode<CharacterBody2D>("../../Player");
	}

	public override void _Process(double delta) {
		staminaBar.Value = player.GetMeta("stamina").AsInt32();
	}
}
