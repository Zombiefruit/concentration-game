using Godot;
using System;

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
}


public partial class GameState : Node
{
	private Card[] _deck;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateDeck();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GenerateDeck()
	{
		// We want to generate a random array of sets of two cards, designated by a suit and a rank.
		// We will use this array to create a deck of cards.
		_deck = new Card[10];

		for (int i = 0; i < _deck.Length; i++)
		{
			_deck[i].suit = (Suit)(GD.Randi() % 4);
			_deck[i].rank = (int)(GD.Randi() % 12) + 1;
		}
	}

	public Card[] GetDeck()
	{
		return _deck;
	}
}
