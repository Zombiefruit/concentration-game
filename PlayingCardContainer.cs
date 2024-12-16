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
	public delegate void CardFlippedEventHandler(int rank, string suit, ulong id);

	public bool cardIsFlipped = false;
	public bool allowClicks = true;
	public ulong id;
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _cardFlipSoundPlayer;
	private AudioStreamPlayer2D _cardHoverSoundPlayer;
	private ShaderMaterial _cardMaterial;
	private AnimatedSprite2D _cardSprite;
	private ShaderMaterial _highlightShader = ResourceLoader.Load("res://shaders/highlight-material.tres") as ShaderMaterial;
	private ShaderMaterial _rippleShader = ResourceLoader.Load("res://shaders/ripple-material.tres") as ShaderMaterial;
	private AnimatedSprite2D _shadow;

	#region Tweens
	private Tween _hoverTween;
	private Tween _rotateTween;
	private Tween _shadowTween;
	#endregion


	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("%CardAnimationPlayer");
		_cardFlipSoundPlayer = GetNode<AudioStreamPlayer2D>("%CardFlipSound");
		_cardHoverSoundPlayer = GetNode<AudioStreamPlayer2D>("%HoverSound");

		_cardMaterial = GetNode<CollisionShape2D>("%CardCollisionShape").Material as ShaderMaterial;
		_cardSprite = GetNode<AnimatedSprite2D>("%PlayingCard");
		SpriteFrames spriteFrames = new SpriteFrames();
		spriteFrames.AddAnimation("card_flip");
		spriteFrames.AddFrame("card_flip", backOfCardTexture);
		spriteFrames.AddFrame("card_flip", frontOfCardTexture);

		_cardSprite.SpriteFrames = spriteFrames;
		_cardSprite.Animation = "card_flip";
		_cardSprite.Frame = 0;

		id = GetInstanceId();
		_shadow = GetNode<AnimatedSprite2D>("%Shadow");
	}

	public override void _Process(double delta)
	{
		HandleShadow();
	}

	private void OnArea2DMouseEntered()
	{
		if (_hoverTween != null && _hoverTween.IsRunning())
			_hoverTween.Kill();

		if (_cardHoverSoundPlayer.Playing)
			_cardHoverSoundPlayer.Stop();

		_cardHoverSoundPlayer.Play();


		_hoverTween = GetTree().CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic).SetParallel(true);
		_hoverTween.TweenProperty(_cardSprite, "scale", new Vector2(1.05f, 1.05f), 0.5f);
		// slightly larger shadow, to give the illusion of a card hovering
		_hoverTween.TweenProperty(_shadow, "scale", new Vector2(1.03f, 1.03f), 0.5f);

		if (_rotateTween != null && _rotateTween.IsRunning())
			_rotateTween.Kill();

		_rotateTween = GetTree().CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
		_rotateTween.TweenProperty(this, "rotation", 0.03, 0.15f);
		_rotateTween.TweenProperty(this, "rotation", -0.02, 0.1f);
		_rotateTween.TweenProperty(this, "rotation", 0.00, 0.1f);
	}

	private void OnArea2DMouseExited()
	{
		if (_rotateTween != null && _rotateTween.IsRunning())
			_rotateTween.Kill();

		_rotateTween = GetTree().CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel(true);
		_rotateTween.TweenProperty(this, "rotation", 0.0f, 0.5f);

		if (_hoverTween != null && _hoverTween.IsRunning())
			_hoverTween.Kill();

		_hoverTween = GetTree().CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic).SetParallel(true);
		_hoverTween.TweenProperty(_cardSprite, "scale", Vector2.One, 0.55f);
		_hoverTween.TweenProperty(_shadow, "scale", Vector2.One, 0.55f);
	}

	void HandleShadow()
	{
		Vector2 center = GetViewportRect().Size / 2.0f;

		float distance = GlobalPosition.X - center.X;

		// Update the shadow's X position based on the distance to give some depth effect - copied from Balatro Godot tutorial
		_shadow.Position = new Vector2(
			Mathf.Lerp(0.0f, -Mathf.Sign(distance) * 10, Mathf.Abs(distance / center.X)),
			_shadow.Position.Y
		);
	}


	public void OnInputEvent(Viewport _viewport, InputEvent @event, int _shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && allowClicks)
		{
			GD.Print("mouse button event at ", mouseEvent.Position);

			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				GD.Print("Mouse button pressed");
				ToggleCardFlip();
				EmitSignal(SignalName.CardFlipped, rank, suit.ToString(), id);
			}
		}
	}

	public void FlipUp()
	{
		_cardFlipSoundPlayer.Play();
		cardIsFlipped = true;
		_animationPlayer.Play("flip_card");
	}

	public void FlipDown()
	{
		_cardFlipSoundPlayer.Play();
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
