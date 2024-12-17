using Godot;
using System;

public partial class CountdownTimerContainer : HBoxContainer
{
	public int timeLimitInSeconds; // The time limit in seconds
	private float remainingTime; // Time left in the countdown

	private bool _isRunning = false;
	private string _lastText = "%s";

	private Label _seconds;
	private Label _milliSeconds;

	public override void _Ready()
	{
		remainingTime = timeLimitInSeconds;
		_seconds = GetNode<Label>("Seconds");
		_milliSeconds = GetNode<Label>("MilliSeconds");
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

		_isRunning = true;
	}


	private void UpdateCountdownText()
	{
		int seconds = (int)(remainingTime % 60);
		int milliseconds = (int)((remainingTime - Math.Floor(remainingTime)) * 100);

		_seconds.Text = $"{seconds:00}";
		_milliSeconds.Text = $"{milliseconds:00}";
	}
}

