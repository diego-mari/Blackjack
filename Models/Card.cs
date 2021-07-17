using System;

namespace Blackjack.Models
{
    public class Card
    {
        char _suit; public char Suit => _suit;
        string _face; public string Face => _face;

        int _value; public int Value { get => _value; set => _value = value; }

        public Card(char suit, string face) // standard constructor.
        {
            _suit = suit;
            _face = face;

            _value = _face switch
            {
                "A" => 11,
                "J" or "Q" or "K" => 10,
                _ => Convert.ToInt32(_face)
            };
        }

        public override string ToString()
            => _face + " " + _suit;
    }
}