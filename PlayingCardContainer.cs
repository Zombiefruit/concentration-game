using Godot;
using System;

public partial class PlayingCardContainer : Node2D
{
	[Export]
	public Texture2D backOfCardTexture;

	[Export]
	public Texture2D frontOfCardTexture;

	private bool _wasClicked = false;
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
		// Connect("OnArea2DMouseEntered", new Callable(this, nameof(OnMouseEntered)));
		// Connect("OnArea2DMouseExited", new Callable(this, nameof(OnMouseExited)));
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
		// _cardSprite.Play("card_flip");
		// _cardSprite.Material = _highlightShader;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void OnArea2DMouseEntered()
	{
		Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		// _isHovered = true;
		// _hoverTime = 0.01f;  // Reset the time when hover begins
		// _highlightShader.SetShaderParameter("hover_time_offset", _hoverTime);
		// _cardSprite.Material = _highlightShader;

	}

	private void OnArea2DMouseExited()
	{
		Input.SetDefaultCursorShape(Input.CursorShape.Arrow); // Reset to default
															  // _isHovered = false;
															  // _hoverTime = 0.0f;  // Optionally reset time when hover ends
															  // _highlightShader.SetShaderParameter("hover_time_offset", _hoverTime);
															  // _cardSprite.Material = null;
	}

	private void OnInputEvent(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseButton && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GD.Print("Left mouse button pressed");

			// _cardSprite.Material = _rippleShader;

			if (!_wasClicked)
			{
				_wasClicked = true;
				_animationPlayer.Play("flip_card");
				_audioStreamPlayer.Play();

			}
			else
			{
				_wasClicked = false;
				_animationPlayer.Play("flip_card_back");
				_audioStreamPlayer.Play();
			}
		}
	}
}
