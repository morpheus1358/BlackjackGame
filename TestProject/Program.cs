using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackGame
{
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
            return Rank switch
            {
                "J" or "Q" or "K" => 10,
                "A" => 11,
                _ => int.Parse(Rank)
            };
        }

        public override string ToString() => $"{Rank} of {Suit}";
    }

    public class Deck
    {
        private List<Card> cards = new();
        private Random random = new();

        public Deck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (string suit in suits)
                foreach (string rank in ranks)
                    cards.Add(new Card(suit, rank));
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public Card DealCard()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }

    public class Player
    {
        public string Name { get; }
        public List<Card> Hand { get; } = new();

        public Player(string name)
        {
            Name = name;
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public int GetHandValue()
        {
            int value = Hand.Sum(card => card.GetValue());
            int aceCount = Hand.Count(card => card.Rank == "A");

            while (value > 21 && aceCount > 0)
            {
                value -= 10; // treat an Ace as 1 instead of 11
                aceCount--;
            }

            return value;
        }

        public void ShowHand(bool revealAll = true)
        {
            Console.WriteLine($"{Name}'s Hand:");
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Name == "Dealer" && !revealAll && i == 0)
                    Console.WriteLine("Hidden card");
                else
                    Console.WriteLine(Hand[i]);
            }
            if (revealAll)
                Console.WriteLine($"Total Value: {GetHandValue()}\n");
        }
    }

    class Program
    {
        static void Main()
        {
            bool playAgain = true;

            while (playAgain)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("♠ Welcome to Blackjack ♣\n");
                Console.ResetColor();

                Deck deck = new();
                deck.Shuffle();

                Player player = new("Player");
                Player dealer = new("Dealer");

                // Initial deal
                player.AddCard(deck.DealCard());
                dealer.AddCard(deck.DealCard());
                player.AddCard(deck.DealCard());
                dealer.AddCard(deck.DealCard());

                // Show starting hands
                player.ShowHand();
                dealer.ShowHand(revealAll: false);

                // Player turn
                bool playerTurn = true;
                while (playerTurn)
                {
                    Console.Write("Hit or stand? (h/s): ");
                    string choice = Console.ReadLine()?.ToLower() ?? "";

                    if (choice == "h")
                    {
                        player.AddCard(deck.DealCard());
                        player.ShowHand();

                        if (player.GetHandValue() > 21)
                        {
                            Console.WriteLine("You bust! 💥 Dealer wins.\n");
                            playerTurn = false;
                            goto EndRound;
                        }
                    }
                    else if (choice == "s")
                    {
                        playerTurn = false;
                    }
                }

                // Dealer turn
                Console.WriteLine("\nDealer's turn...");
                dealer.ShowHand(revealAll: true);

                while (dealer.GetHandValue() < 17)
                {
                    Console.WriteLine("Dealer hits...");
                    dealer.AddCard(deck.DealCard());
                    dealer.ShowHand();
                    System.Threading.Thread.Sleep(800);
                }

                if (dealer.GetHandValue() > 21)
                {
                    Console.WriteLine("Dealer busts! 🎉 You win!\n");
                }
                else
                {
                    Console.WriteLine("\nFinal Results:");
                    Console.WriteLine($"Your total: {player.GetHandValue()}");
                    Console.WriteLine($"Dealer total: {dealer.GetHandValue()}");

                    if (player.GetHandValue() > dealer.GetHandValue())
                        Console.WriteLine("You win! 🎉\n");
                    else if (player.GetHandValue() < dealer.GetHandValue())
                        Console.WriteLine("Dealer wins! 💀\n");
                    else
                        Console.WriteLine("It's a tie! 🤝\n");
                }

            EndRound:
                Console.Write("Play again? (y/n): ");
                playAgain = Console.ReadLine()?.ToLower() == "y";
            }

            Console.WriteLine("\nThanks for playing! 🃏");
        }
    }
}
