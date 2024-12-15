using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public partial class Main : Node2D
{
	private GridContainer _cards;
	private GameState _gameState;
	private PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://playing_card.tscn");

	private int _numberOfCardsFlipped = 0;

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

			playingCardContainer.Connect("CardFlipped", new Callable(this, nameof(OnCardFlipped)));
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

	private async void OnCardFlipped(int rank, string suit)
	{
		GD.Print($"Card flipped: {rank} of {suit}");
		_numberOfCardsFlipped++;

		if (_numberOfCardsFlipped == 2)
		{
			_cards.MouseFilter = Control.MouseFilterEnum.Stop;
			List<PlayingCardContainer> flippedCards = new List<PlayingCardContainer>();

			foreach (PlayingCardContainer card in _cards.GetChildren())
			{
				if (card.cardIsFlipped)
				{
					flippedCards.Add(card);
				}
			}


			// Check if the two flipped cards match
			// If they match, remove them from the grid
			// If they don't match, flip them back over
			bool match = _gameState.CheckMatch(new Card
			{
				rank = flippedCards[0].rank,
				suit = flippedCards[0].suit
			}, new Card
			{
				rank = flippedCards[1].rank,
				suit = flippedCards[1].suit
			});



			// wait for two seconds
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);


			_numberOfCardsFlipped = 0;

			foreach (PlayingCardContainer card in flippedCards)
			{
				card.FlipDown();
			}

			if (match)
			{

				foreach (PlayingCardContainer card in flippedCards)
				{
					card.Modulate = new Color(1, 1, 1, 0);
					card.allowClicks = false;
					card.MouseDefaultCursorShape = Control.CursorShape.Arrow;
				}
			}

			_cards.MouseFilter = Control.MouseFilterEnum.Pass;
			// cards.GetTree().CallGroup("PlayingCards", "FlipDown");


			// GetTree().CallGroup("PlayingCards", "FlipDown");

		}
	}
}
