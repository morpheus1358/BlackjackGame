// See https://aka.ms/new-console-template for more information
using System.Globalization;


public class Card
{
    public string Suit { get; }
    public string Rank { get; }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public int GetValue()
    {
        if (Rank == "J" || Rank == "Q" || Rank == "K")
            return 10;
        else if (Rank == "A")
            return 11; // handle Ace later (can be 1 or 11)
        else
            return int.Parse(Rank);
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
public class Deck
{
    private List<Card> cards;
    private Random random = new Random();

    public Deck()
    {
        cards = new List<Card>();
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                cards.Add(new Card(suit, rank));
            }
        }
    }
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    public Card DealCard()
    {
        if (cards.Count == 0) throw new InvalidOperationException("No cards left in the deck!");
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}

public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; } = new List<Card>();

    public Player(string name)
    {
        Name = name;
    }

    public void AddCard(Card card)
    {
        Hand.Add(card);
    }

    public int GetScore()
    {
        int score = 0;
        int aceCount = 0;

        foreach (var card in Hand)
        {
            int value = card.GetValue();
            score += value;
            if (card.Rank == "A") aceCount++;
        }

        // Adjust Aces from 11 → 1 if we bust
        while (score > 21 && aceCount > 0)
        {
            score -= 10; // turn an Ace from 11 into 1
            aceCount--;
        }

        return score;
    }

    public void ShowHand(bool showAllCards = true)
    {
        Console.WriteLine($"{Name}'s hand:");

        for (int i = 0; i < Hand.Count; i++)
        {
            if (!showAllCards && i == 1) // dealer’s hidden card
                Console.WriteLine("Hidden card");
            else
                Console.WriteLine(Hand[i]);
        }

        if (showAllCards)
            Console.WriteLine($"Total: {GetScore()}");
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        Deck deck = new Deck();
        deck.Shuffle();

        Player player = new Player("Player");
        Player dealer = new Player("Dealer");

        // Initial deal
        player.AddCard(deck.DealCard());
        player.AddCard(deck.DealCard());
        dealer.AddCard(deck.DealCard());
        dealer.AddCard(deck.DealCard());

        player.ShowHand();
        dealer.ShowHand(showAllCards: false);
    }
}

