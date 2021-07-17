using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Blackjack.Models
{
    public class Game
    {
        char[] suit = {'♥', '♦', '♣', '♠'};
        string[] face = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};

        List<Card> cards = new List<Card>();

        public void FillDeck()
        {
            foreach(var s in suit)
                foreach(var f in face)
                    cards.Add(new Card(s, f));

            ShuffleDeck();
        }

        void ShuffleDeck()
        {
            for(int i = 0; i < cards.Count; i++)
            {
                int j = new Random().Next(cards.Count);
                Card tmp = cards[i];
                cards[i] = cards[j];
                cards[j] = tmp;
            }
        }

        public string Deck()
        {
            StringBuilder sb = new StringBuilder();

            foreach(var c in cards)
                sb.AppendLine(c.ToString());

            return sb.ToString();
        }

        string file = "Models/credits.txt"; // file-path.

        public int Credits() // recovers credits from a .txt file.
        {
            try
            {
                string[] words = File.ReadAllText(file).Split(' ');
                return Convert.ToInt32(words[1]);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SaveCredits(int credits) // saves credits on a .txt file.
            => File.WriteAllText(file, "$ " + credits);

        List<Card> cardsP = new List<Card>(); public int valueP = 0; // player.
        public List<Card> cardsD = new List<Card>(); public int valueD = 0; // dealer.

        public string Deal(int credits, int bet)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < 4; i++)
            {
                if(i % 2 == 0)
                {
                    if(valueP + cards[i].Value > 21)
                        cards[i].Value = 1;

                    valueP += cards[i].Value;
                    cardsP.Add(cards[i]);
                }
                else
                {
                    if(valueD + cards[i].Value > 21)
                        cards[i].Value = 1;

                    valueD += cards[i].Value;
                    cardsD.Add(cards[i]);
                }
            }

            for(int i = 0; i < cardsP.Count; i++)
            {
                sb.Append(cardsP[i].ToString());
                if(i != cardsP.Count - 1)
                    sb.Append(" / ");
            }

            sb.AppendLine($"\nPlayer's Value: {valueP}\n");

            if(valueP != 21 && cardsD[0].Face == "A" && credits >= bet / 2 || valueP != 21 && valueD != 21)
                sb.Append(cardsD[0].ToString());
            else
            {
                for(int i = 0; i < cardsD.Count; i++)
                {
                    sb.Append(cardsD[i].ToString());
                    if(i != cardsD.Count - 1)
                        sb.Append(" / ");
                }
            }

            sb.Append("\nDealer's Value: ");
            if(valueP != 21 && cardsD[0].Face == "A" && credits >= bet / 2 || valueP != 21 && valueD != 21)
                sb.AppendLine(cardsD[0].Value.ToString());
            else
                sb.AppendLine(valueD.ToString());

            return sb.ToString();
        }

        public int Insurance(int credits, int bet)
        {
            StringBuilder sb = new StringBuilder();

            if(cardsD[0].Face == "A" && credits >= bet / 2) // player can buy insurance.
            {
                string ans;
                List<string> opt = new List<string>{"yes", "no"};

                Console.Write("\n");

                do
                {
                    Console.Write("Insure [yes/no]? ");
                    ans = Console.ReadLine();

                    ans = ans switch
                    {
                        "y" => "yes",
                        "n" => "no",
                        _ => ans
                    };
                }while(! opt.Contains(ans));
                Console.Clear();
            
                for(int i = 0; i < cardsP.Count; i++)
                {
                    sb.Append(cardsP[i].ToString());
                    if(i != cardsP.Count - 1)
                        sb.Append(" / ");
                }

                sb.AppendLine($"\nPlayer's Value: {valueP}\n");
                if(valueD != 21)
                    sb.Append(cardsD[0].ToString());
                else
                    for(int i = 0; i < cardsD.Count; i++)
                    {
                        sb.Append(cardsD[i].ToString());
                        if(i != cardsD.Count - 1)
                            sb.Append(" / ");
                    }    

                sb.Append("\nDealer's Value: ");
                if(valueD != 21)
                    sb.AppendLine(cardsD[0].Value.ToString());
                else
                    sb.Append(valueD.ToString());

                Console.WriteLine(sb.ToString());

                if(valueD == 21)
                {
                    return ans switch
                    {
                        "yes" => bet,
                        _ => - (bet / 2)
                    };
                }

                if(ans == "yes")
                    return - (bet / 2);
            }

            return 0;
        }

        public void Hit(string ans)
        {
            StringBuilder sb = new StringBuilder();

            valueP += cards[cardsP.Count + cardsD.Count].Value;
            cardsP.Add(cards[cardsP.Count + cardsD.Count]);

            if(valueP > 21)
            {
                int tmp = 0;

                foreach(var c in cardsP)
                {
                    if(c.Face == "A" && c.Value == 11)
                    {
                        c.Value = 1;
                        break;
                    }
                }

                foreach(var c in cardsP)
                    tmp += c.Value;

                valueP = tmp;
            }

            if(ans != "double" && valueP < 21 || valueP > 21)
            {
                for(int i = 0; i < cardsP.Count; i++)
                {
                    sb.Append(cardsP[i].ToString());
                    if(i != cardsP.Count - 1)
                        sb.Append(" / ");
                }

                sb.AppendLine($"\nPlayer's Value: {valueP}\n");
                if(valueP < 21 && cardsD[0].Face != "A")
                    sb.Append(cardsD[0].ToString());
                else
                    for(int i = 0; i < cardsD.Count; i++)
                    {
                        sb.Append(cardsD[i].ToString());
                        if(i != cardsD.Count - 1)
                            sb.Append(" / ");
                    }    

                sb.Append("\nDealer's Value: ");
                if(valueP < 21)
                    sb.AppendLine(cardsD[0].Value.ToString());
                else
                    sb.AppendLine(valueD.ToString());

                Console.Write(sb.ToString());
            }

            if(valueP > 21)
                throw new Exception("\nBUSTED!");
        }

        public int Result(int bet, string ans)
        {
            StringBuilder sb = new StringBuilder();

            if(ans != "fold")
                for (int i = cardsP.Count + cardsD.Count; valueD < 17; i++)
                {
                    valueD += cards[i].Value;
                    cardsD.Add(cards[i]);

                    if(valueD > 21)
                    {
                        int tmp = 0;

                        foreach(var c in cardsD)
                        {
                            if(c.Face == "A" && c.Value == 11)
                            {
                                c.Value = 1;
                                break;
                            }
                        }

                        foreach(var c in cardsD)
                            tmp += c.Value;

                        valueD = tmp;
                    }
                }
        
            for(int i = 0; i < cardsP.Count; i++)
            {
                sb.Append(cardsP[i].ToString());
                if(i != cardsP.Count - 1)
                    sb.Append(" / ");
            }

            sb.AppendLine($"\nPlayer's Value: {valueP}\n");
            for(int i = 0; i < cardsD.Count; i++)
            {
                sb.Append(cardsD[i].ToString());
                if(i != cardsD.Count - 1)
                    sb.Append(" / ");
            }   

            sb.Append("\nDealer's Value: " + valueD);
            Console.WriteLine(sb.ToString());

            if(ans == "fold")
                return bet / 2;

            if(valueD > 21 || valueP > valueD)
            {
                Win();

                if(ans == "double")
                    bet *= 2;

                return bet * 2;
            }
            else if(valueP < valueD)
            {
                Lose();
                return 0;
            }

            Push();

            if(ans == "double")
                bet *= 2;

            return bet;
        }

        public void Win()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nYOU WON!");
            Console.ResetColor();
        }

        public void Push()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPUSH!");
            Console.ResetColor();
        }

        public void Lose()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nYOU LOST!");
            Console.ResetColor();
        }
    }
}