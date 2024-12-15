using Godot;
using System;


public partial class PlayingCardContainer : Control
{
	[Export]
	public Texture2D backOfCardTexture;

	[Export]
	public Texture2D frontOfCardTexture;

	[Export]
	public Suit suit;

	[Export]
	public int rank;

	private bool _cardIsFlipped = false;
	private bool _isFlipping = false;
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;
	private ShaderMaterial _cardMaterial;
	private AnimatedSprite2D _cardSprite;
	private ShaderMaterial _highlightShader = ResourceLoader.Load("res://shaders/highlight-material.tres") as ShaderMaterial;
	private ShaderMaterial _rippleShader = ResourceLoader.Load("res://shaders/ripple-material.tres") as ShaderMaterial;

	private float _hoverTime = 0.0f;
	private bool _isHovered = false;


	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("%CardAnimationPlayer");
		_audioStreamPlayer = GetNode<AudioStreamPlayer2D>("%CardFlipSound");

		_cardMaterial = GetNode<CollisionShape2D>("%CardCollisionShape").Material as ShaderMaterial;
		_cardSprite = GetNode<AnimatedSprite2D>("%PlayingCard");
		SpriteFrames spriteFrames = new SpriteFrames();
		spriteFrames.AddAnimation("card_flip");
		spriteFrames.AddFrame("card_flip", backOfCardTexture);
		spriteFrames.AddFrame("card_flip", frontOfCardTexture);

		_cardSprite.SpriteFrames = spriteFrames;
		_cardSprite.Animation = "card_flip";
		_cardSprite.Frame = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void OnArea2DMouseEntered()
	{
		GD.Print("Mouse entered");
		Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
	}

	private void OnArea2DMouseExited()
	{
		GD.Print("Mouse exited");
		Input.SetDefaultCursorShape(Input.CursorShape.Arrow); // Reset to default
	}

	// private void OnInputEvent(Viewport viewport, InputEvent @event, int shape_idx)
	// {
	// 	if (Input.IsMouseButtonPressed(MouseButton.Left))
	// 	{
	// 		GD.Print("Mouse button pressed");
	// 		ToggleCardFlip();
	// 	}
	// }

	public void OnInputEvent(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			GD.Print("mouse button event at ", mouseEvent.Position);

			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				GD.Print("Mouse button pressed");
				ToggleCardFlip();
			}
		}
	}

	private void ToggleCardFlip()
	{
		if (!_cardIsFlipped)
		{
			GD.Print("Flipping card, since it is not flipped");
			_audioStreamPlayer.Play();
			_cardIsFlipped = true;
			_animationPlayer.Play("flip_card");


		}
		else
		{
			GD.Print("Flipping card back, since it is flipped");
			_audioStreamPlayer.Play();
			_cardIsFlipped = false;
			_animationPlayer.Play("flip_card_back");
		}
	}
}
