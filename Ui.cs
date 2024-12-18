using Godot;
using System;

public partial class Ui : Control
{
    private AudioStreamPlayer _playButtonSound;
    public override void _Ready()
    {
        _playButtonSound = GetNode<AudioStreamPlayer>("%PlayButtonSound");
    }

    public void OnPlayButtonMouseEntered()
    {
        _playButtonSound.PitchScale = 1.0f;
        _playButtonSound.Play(0.8f);
    }

    public void OnDifficultySelectMouseEntered()
    {
        _playButtonSound.PitchScale = 0.9f;
        _playButtonSound.Play(0.8f);
    }
}
