using Godot;
using System.Collections.Generic;

public enum Difficulty
{
	Baby,
	BiggerBaby,
	BiggestBaby,
	Toddler
}

public enum Suit
{
	Hearts,
	Diamonds,
	Clubs,
	Spades
}


public struct Card
{
	public Suit suit;
	public int rank;
	public ulong id;
}


public partial class GameState : Node
{

	[Signal]
	public delegate void RoundWonEventHandler();

	[Signal]
	public delegate void ScoreUpdatedEventHandler(int newScore);

	private int _deckSize = 14;
	private int _numberOfCardsFlipped = 0;
	private int _numberOfCardsMatched = 0;
	private int _gameTimeLimitInSeconds = 25;

	private bool _roundOver;

	private Card[] _deck;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_numberOfCardsMatched == _deckSize && !_roundOver)
		{
			GD.Print("Round over!");
			EmitSignal(SignalName.RoundWon);
			_roundOver = true;
		}
	}

	public void Init(Difficulty difficulty)
	{
		SetDeckSize(difficulty);
		SetTimeLimit(difficulty);
		GenerateDeck();
		_roundOver = false;
	}

	#region Getters

	public int GetGameTimeLimit()
	{
		return _gameTimeLimitInSeconds;
	}

	public int GetDeckSize()
	{
		return _deckSize;
	}

	public Card[] GetDeck()
	{
		return _deck;
	}

	#endregion

	public void Reset()
	{
		_numberOfCardsMatched = 0;
		_numberOfCardsFlipped = 0;
		_roundOver = false;
	}

	public bool CheckMatch(Card card1, Card card2)
	{
		if (card1.rank == card2.rank && card1.suit == card2.suit)
		{
			GD.Print("Match!");
			_numberOfCardsMatched += 2;

			EmitSignal(SignalName.ScoreUpdated, _numberOfCardsMatched);
			return true;
		}
		else
		{
			GD.Print("No match!");
			return false;
		}
	}

	public void SetDeckSize(Difficulty difficulty)
	{
		switch (difficulty)
		{
			case Difficulty.Baby:
				_deckSize = 10;
				break;
			case Difficulty.BiggerBaby:
				_deckSize = 14;
				break;
			case Difficulty.BiggestBaby:
				_deckSize = 20;
				break;
			case Difficulty.Toddler:
				_deckSize = 24;
				break;
		}
	}

	public void SetTimeLimit(Difficulty difficulty)
	{
		switch (difficulty)
		{
			case Difficulty.Baby:
				_gameTimeLimitInSeconds = 60;
				break;
			case Difficulty.BiggerBaby:
				_gameTimeLimitInSeconds = 45;
				break;
			case Difficulty.BiggestBaby:
				_gameTimeLimitInSeconds = 40;
				break;
			case Difficulty.Toddler:
				_gameTimeLimitInSeconds = 30;
				break;
		}
	}

	private void GenerateDeck()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		List<Card> allCards = new List<Card>();
		for (int suit = 0; suit < 4; suit++) // 4 suits
		{
			for (int rank = 1; rank <= 13; rank++) // Ranks 1 to 13
			{
				allCards.Add(new Card
				{
					suit = (Suit)suit,
					rank = rank
				});
			}
		}

		// Randomly pick deckSize / 2 unique cards from the full pool
		List<Card> selectedCards = new List<Card>();

		for (int i = 0; i < _deckSize / 2; i++)
		{
			int randomIndex = rng.RandiRange(0, allCards.Count - 1);
			selectedCards.Add(allCards[randomIndex]);
			allCards.RemoveAt(randomIndex); // Ensure no duplicates
		}

		// Duplicate the cards to create pairs
		List<Card> fullDeck = new List<Card>(selectedCards);
		fullDeck.AddRange(selectedCards);

		// Shuffle the final deck
		ShuffleDeck(fullDeck, rng);

		// Assign to the _deck array
		_deck = fullDeck.ToArray();

	}

	private void ShuffleDeck(List<Card> deck, RandomNumberGenerator rng)
	{
		for (int i = deck.Count - 1; i > 0; i--)
		{
			int randomIndex = rng.RandiRange(0, i);
			// Swap deck[i] with deck[randomIndex]
			Card temp = deck[i];
			deck[i] = deck[randomIndex];
			deck[randomIndex] = temp;
		}
	}
}
