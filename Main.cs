using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Control
{
	private GridContainer _cards;
	private GameLayerContainer _gameLayerContainer;
	private GameState _gameState;
	private Control _ui;
	private PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://scenes/playing_card.tscn");
	private CountdownTimerContainer _countdownTimerContainer;

	private int _numberOfCardsFlipped = 0;
	private bool _beginRound = false;

	private float _shuffleInTimeout = 0.35f;
	private AudioStreamPlayer _playButtonSound;
	private List<PlayingCardContainer> _flippedCards = new List<PlayingCardContainer>();
	private OptionButton _difficultySelect;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		_gameLayerContainer = GetNode<GameLayerContainer>("%GameLayerContainer");
		_playButtonSound = GetNode<AudioStreamPlayer>("%PlayButtonSound");
		_gameLayerContainer.Hide();
		_cards = GetNode<GridContainer>("%GameLayerContainer/%Cards");
		_gameState = GetNode<GameState>("%GameState");
		_ui = GetNode<Control>("%UI");

		_difficultySelect = GetNode<OptionButton>("%DifficultySelect");
		SetDifficultySelectOptions(_difficultySelect);

		_gameLayerContainer.Connect("Retry", new Callable(this, nameof(ResetGame)));
		_gameState.Connect("RoundWon", new Callable(this, nameof(RoundWon)));

		_countdownTimerContainer = _gameLayerContainer.GetCountdownTimerContainer();
		_countdownTimerContainer.Connect("TimeExpired", new Callable(this, nameof(TimerExpired)));
	}

	public override void _Process(double delta)
	{
		AnimateDeck();
	}

	public async void OnPlayButtonPressed()
	{
		GD.Print("Play button pressed");
		int selectedDifficultyIndex = _difficultySelect.GetSelectedId();
		Difficulty selectedDifficulty = (Difficulty)selectedDifficultyIndex;
		_gameState.Init(selectedDifficulty);

		SetGridColumns();
		InitDeck();

		_playButtonSound.PitchScale = 0.5f;
		_playButtonSound.Play(0.5f);
		_ui.Hide();
		_gameLayerContainer.Show();
		int gameTimeLimit = _gameState.GetGameTimeLimit();

		await ToSignal(GetTree().CreateTimer(_shuffleInTimeout), SceneTreeTimer.SignalName.Timeout);

		_countdownTimerContainer.Start(gameTimeLimit);
	}

	public void OnPlayButtonMouseEntered()
	{
		_playButtonSound.PitchScale = 1.0f;
		_playButtonSound.Play(0.8f);
	}

	private void SetDifficultySelectOptions(OptionButton difficultySelect)
	{
		difficultySelect.Clear();

		foreach (Difficulty difficulty in Enum.GetValues(typeof(Difficulty)))
		{
			difficultySelect.AddItem(difficulty.ToString());
		}
	}

	private void SetGridColumns()
	{
		int deckSize = _gameState.GetDeckSize();

		if (deckSize < 20)
		{
			_gameLayerContainer.SetCardGridColumns(_gameState.GetDeckSize() / 2);
		}
		else
		{
			_gameLayerContainer.SetCardGridColumns(_gameState.GetDeckSize() / 4);
		}



	}

	private void InitDeck()
	{
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
			// playingCardContainer.Show();
			_cards.AddChild(playingCardContainer);

			playingCardContainer.Connect("CardFlipped", new Callable(this, nameof(OnCardFlipped)));
		}

		_beginRound = true;
	}

	private void AnimateDeck()
	{
		if (_beginRound)
		{
			int i = 0;

			GetNode<AudioStreamPlayer>("%CardShuffleIn").Play(0.9f);

			foreach (PlayingCardContainer card in _cards.GetChildren().OfType<PlayingCardContainer>())
			{
				// make a new tween
				Tween tween = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
				// animate cards from offscreen onto screen
				Vector2 originalPosition = card.Position;
				GD.Print(originalPosition);

				tween.TweenProperty(card, "position", new Vector2(-200, -200), 0.0f);
				tween.TweenProperty(card, "position", originalPosition, _shuffleInTimeout + i * 0.01f);

				i++;
			}

			_beginRound = false;
		}
	}

	private void ResetGame()
	{
		_gameLayerContainer.Hide();
		// _gameLayerContainer.GetCountdownTimerContainer();
		_cards.MouseFilter = Control.MouseFilterEnum.Pass;
		_cards.GetChildren().OfType<PlayingCardContainer>().ToList().ForEach(card => card.BurnCard());
		_flippedCards.Clear();
		_numberOfCardsFlipped = 0;
		_gameState.Reset();
		_ui.Show();

		_cards.GetChildren().OfType<PlayingCardContainer>().ToList().ForEach(card =>
		{
			_cards.RemoveChild(card);
			card.QueueFree();
		});
	}

	private async void RoundWon()
	{
		_countdownTimerContainer.Stop();
		_cards.MouseFilter = Control.MouseFilterEnum.Stop;

		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		_gameLayerContainer.PlayGameWonAnimation();
	}

	private async void TimerExpired()
	{
		_cards.MouseFilter = Control.MouseFilterEnum.Stop;
		GetTree().CallGroup("PlayingCards", "BurnCard");

		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		_gameLayerContainer.PlayGameOverAnimation();
	}

	private async void OnCardFlipped(int rank, string suit, ulong id)
	{
		GD.Print($"Card flipped: {rank} of {suit}");
		_numberOfCardsFlipped++;

		PlayingCardContainer card = InstanceFromId(id) as PlayingCardContainer;
		_flippedCards.Add(card);
		card.allowClicks = false;

		if (_flippedCards.Count >= 2)
		{
			_cards.MouseFilter = Control.MouseFilterEnum.Stop;

			// Check if the two flipped cards match
			// If they match, remove them from the grid
			// If they don't match, flip them back over
			bool match = _gameState.CheckMatch(new Card
			{
				rank = _flippedCards[0].rank,
				suit = _flippedCards[0].suit
			}, new Card
			{
				rank = _flippedCards[1].rank,
				suit = _flippedCards[1].suit
			});

			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

			_numberOfCardsFlipped = 0;

			if (match)
			{
				_flippedCards.ForEach(async card =>
				{
					card.AnimateShakeIn();
					card.BurnCard();
					await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
					card.Modulate = new Color(1, 1, 1, 0);
				});
			}
			else
			{
				_flippedCards.ForEach(card =>
				{
					card.allowClicks = true;
					card.FlipDown();
				});
			}

			_flippedCards.Clear();
			_cards.MouseFilter = Control.MouseFilterEnum.Pass;
		}
	}
}
