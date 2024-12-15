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

	[Signal]
	public delegate void CardFlippedEventHandler(int rank, string suit);

	public bool cardIsFlipped = false;
	public bool allowClicks = true;
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;
	private ShaderMaterial _cardMaterial;
	private AnimatedSprite2D _cardSprite;
	private ShaderMaterial _highlightShader = ResourceLoader.Load("res://shaders/highlight-material.tres") as ShaderMaterial;
	private ShaderMaterial _rippleShader = ResourceLoader.Load("res://shaders/ripple-material.tres") as ShaderMaterial;


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
		// GD.Print("Mouse entered");
	}

	private void OnArea2DMouseExited()
	{
		// GD.Print("Mouse exited");
	}

	public void OnInputEvent(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && allowClicks)
		{
			GD.Print("mouse button event at ", mouseEvent.Position);

			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				GD.Print("Mouse button pressed");
				ToggleCardFlip();
				EmitSignal(SignalName.CardFlipped, rank, suit.ToString());
			}
		}
	}

	public void FlipUp()
	{
		_audioStreamPlayer.Play();
		cardIsFlipped = true;
		_animationPlayer.Play("flip_card");
	}

	public void FlipDown()
	{
		_audioStreamPlayer.Play();
		cardIsFlipped = false;
		_animationPlayer.Play("flip_card_back");
	}

	private void ToggleCardFlip()
	{
		if (!cardIsFlipped)
		{
			GD.Print("Flipping card, since it is not flipped");
			FlipUp();
		}
		else
		{
			GD.Print("Flipping card back, since it is flipped");
			FlipDown();
		}
	}
}
