using Godot;
using System;
public partial class CountdownTimerContainer : HBoxContainer
{
	[Signal]
	public delegate void TimeExpiredEventHandler();
	public int timeLimitInSeconds; // The time limit in seconds
	private float remainingTime; // Time left in the countdown

	private bool _isRunning = false;
	private string _lastText = "%s";

	private Label _seconds;
	private Label _milliSeconds;
	private Label _colon;
	private int _frameCounter = 0;
	private Vector2 _originalSecondsPosition = new Vector2();
	private Vector2 _originalMilliSecondsPosition = new Vector2();
	private Vector2 _originalColonPosition = new Vector2();
	private AudioStreamPlayer _countdownSoundPlayer;

	public override void _Ready()
	{
		remainingTime = timeLimitInSeconds;
		_seconds = GetNode<Label>("Seconds");
		_milliSeconds = GetNode<Label>("MilliSeconds");
		_colon = GetNode<Label>("Colon");
		_countdownSoundPlayer = GetNode<AudioStreamPlayer>("CountdownSound");
	}

	public override void _Process(double delta)
	{
		if (remainingTime > 0 && _isRunning)
		{
			remainingTime -= (float)delta;

			if (remainingTime < 0) remainingTime = 0;

			UpdateCountdownText();
		}
	}

	public void Start(int timeLimitInSeconds)
	{
		this.timeLimitInSeconds = timeLimitInSeconds;
		remainingTime = timeLimitInSeconds;

		_seconds.AddThemeColorOverride("font_color", new Color("#FFF"));
		_milliSeconds.AddThemeColorOverride("font_color", new Color("#FFF"));
		_colon.AddThemeColorOverride("font_color", new Color("#FFF"));

		_isRunning = true;
	}

	public void Stop()
	{
		_isRunning = false;
		_countdownSoundPlayer.Stop();
	}

	public void Reset()
	{
		remainingTime = timeLimitInSeconds;
		_isRunning = false;
		UpdateCountdownText();
	}


	private void UpdateCountdownText()
	{
		int seconds = (int)(remainingTime % 60);
		int milliseconds = (int)((remainingTime - Math.Floor(remainingTime)) * 100);

		_seconds.Text = $"{seconds:00}";
		_milliSeconds.Text = $"{milliseconds:00}";


		// start the shake effect
		ShakeText(0.3f);

		if (remainingTime < 25 && _countdownSoundPlayer.Playing == false)
		{
			_countdownSoundPlayer.Play();
		}

		if (remainingTime / timeLimitInSeconds < 0.5 && remainingTime / timeLimitInSeconds > 0.25)
		{
			// increase the shake effect
			ShakeText(0.7f);
		}
		else if (remainingTime / timeLimitInSeconds < 0.25 && remainingTime / timeLimitInSeconds > 0.1)
		{
			// increase the shake effect
			ShakeText(1.2f, true);
		}
		else if (remainingTime / timeLimitInSeconds < 0.1)
		{
			// increase the shake effect
			ShakeText(1.8f, true);
		}

		if (remainingTime <= 0)
		{
			_isRunning = false;
			_countdownSoundPlayer.Stop();
			_seconds.Text = "00";
			_milliSeconds.Text = "00";
			EmitSignal(SignalName.TimeExpired);
		}

	}

	private void ShakeText(float strength, bool shouldChangeColor = false)
	{
		_originalSecondsPosition = _frameCounter == 0 ? _seconds.Position : _originalSecondsPosition;
		_originalMilliSecondsPosition = _frameCounter == 0 ? _milliSeconds.Position : _originalMilliSecondsPosition;
		_originalColonPosition = _frameCounter == 0 ? _colon.Position : _originalColonPosition;

		if (shouldChangeColor)
		{
			_seconds.AddThemeColorOverride("font_color", new Color("#FFFF33"));
			_milliSeconds.AddThemeColorOverride("font_color", new Color("#FFFF33"));
			_colon.AddThemeColorOverride("font_color", new Color("#FFFF33"));
		}

		if (_frameCounter % 5 == 0)
		{
			_seconds.Position = _originalSecondsPosition;
			_milliSeconds.Position = _originalMilliSecondsPosition;
			_colon.Position = _originalColonPosition;
		}
		else
		{
			_seconds.Position += new Vector2(0, GD.RandRange(-1, 1) * strength);
			_milliSeconds.Position += new Vector2(0, GD.RandRange(-1, 1) * strength);
			_colon.Position += new Vector2(0, GD.RandRange(-1, 1) * strength);
		};

		_frameCounter++;
	}
}

