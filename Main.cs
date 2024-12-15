using Godot;
using System;

public partial class Main : Node2D
{
	private GridContainer _cards;
	private GameState _gameState;
	private PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://playing_card.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		_cards = GetNode<GridContainer>("%Cards");
		_gameState = GetNode<GameState>("GameState");

		Array.ForEach(_gameState.GetDeck(), card => GD.Print(card.suit + " " + card.rank));

		foreach (Card card in _gameState.GetDeck())
		{
			PlayingCardContainer playingCardContainer = packedScene.Instantiate() as PlayingCardContainer;
			playingCardContainer.suit = card.suit;
			playingCardContainer.rank = card.rank;

			string cardRank = card.rank switch
			{
				1 => "A",
				11 => "J",
				12 => "Q",
				13 => "K",
				_ => card.rank.ToString()
			};

			playingCardContainer.backOfCardTexture = ResourceLoader.Load("res://assets/Boardgame Pack/PNG/Cards/cardBack_blue4.png") as Texture2D;
			playingCardContainer.frontOfCardTexture = ResourceLoader.Load($"res://assets/Boardgame Pack/PNG/Cards/card{card.suit}{cardRank}.png") as Texture2D;
			playingCardContainer.Show();
			_cards.AddChild(playingCardContainer);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPlayButtonPressed()
	{
		GetNode<Button>("%PlayButton").Hide();
		GetNode<GridContainer>("%Cards").Show();
	}
}
