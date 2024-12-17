using Godot;
using System;

public partial class GameLayerContainer : CanvasLayer
{
	[Signal]
	public delegate void RetryEventHandler();
	private CountdownTimerContainer _countdownTimerContainer;
	private AnimationPlayer _textAnimator;
	private Control _retryButton;
	private RichTextLabel _gameOverText;
	private RichTextLabel _winnerText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_textAnimator = GetNode<AnimationPlayer>("%TextAnimator");
		_countdownTimerContainer = GetNode<CountdownTimerContainer>("%CountdownTimerContainer");
		_retryButton = GetNode<Control>("%RetryButtonContainer");
		_gameOverText = GetNode<RichTextLabel>("%GameOverText");
		_winnerText = GetNode<RichTextLabel>("%WinnerText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnRetryButtonPressed()
	{
		_retryButton.Hide();
		_gameOverText.Hide();
		_winnerText.Hide();
		_textAnimator.Seek(0);
		EmitSignal(SignalName.Retry);
	}

	public CountdownTimerContainer GetCountdownTimerContainer()
	{
		return _countdownTimerContainer;
	}

	public void PlayGameOverAnimation()
	{
		_textAnimator.Seek(0);
		_textAnimator.Play("game_over_fade_in");
		_gameOverText.Show();
		_retryButton.Show();
	}

	public void PlayGameWonAnimation()
	{
		_textAnimator.Seek(0);
		_textAnimator.Play("winner_fade_in");
		_winnerText.Show();
		_retryButton.Show();
	}
}
