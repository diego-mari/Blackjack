using System;
using System.Text;
using System.Collections.Generic;
using Blackjack.Models;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // console charset.
            Console.Clear();

            try
            {
                Game bj = new Game();
                bj.SaveCredits(Play(bj));
            }
            catch(Exception e)
            {
                ExcText(e);
            }
        }

        static int Play(Game bj)
        {
            const int minBet = 10;
            const int maxBet = 6000;

            int credits = bj.Credits();
            Console.WriteLine($"Current Credits: $ {credits.ToString("N0")} | Minimum Bet: $ {minBet.ToString("N0")} | Maximum Bet: $ {maxBet.ToString("N0")}\n");

            if(credits >= minBet)
            {
                bj.FillDeck();
                // Console.WriteLine(bj.Deck());

                int bet = 0;
                do
                {
                    Console.Write("Place Your Bet: $ ");

                    try { bet = Convert.ToInt32(Console.ReadLine()); }
                    catch { }
                }while(bet < minBet || bet > credits || bet > maxBet);
                
                Console.Clear();

                bj.SaveCredits(credits -= bet);
                bool fDouble = (credits < bet) ? true : false; // checks if the player has enough credits to double.
                Console.Write(bj.Deal(credits, bet));

                if(bj.valueP != 21 && bj.valueD != 21 || bj.valueP != 21 && bj.cardsD[0].Face == "A")
                {
                    credits += bj.Insurance(credits, bet);

                    if(bj.valueD == 21) // dealer had a blackjack.
                        bj.Lose();
                    else
                    {
                        fDouble = (credits < bet) ? true : false; // re-checks if the player has enough credits to double.

                        string ans;
                        List<string> opt = new List<string>{"double", "stand", "hit", "fold"};
                        bool fHit = false;

                        do
                        {
                            Console.WriteLine();
                            do
                            {

                                Console.Write("Choice [");
                                if(! fDouble && ! fHit)
                                    Console.Write("double/");

                                Console.Write("stand/hit");
                                if(! fHit)
                                    Console.Write("/fold");
                                Console.Write("]: ");
                                
                                ans = Console.ReadLine().ToLower();

                                ans = ans switch
                                {
                                    "d" => "double",
                                    "s" => "stand",
                                    "h" => "hit",
                                    "f" => "fold",
                                    _ => ans
                                };
                            }while(! opt.Contains(ans) || fHit && ans == "double" || fDouble && ans == "double" || fHit && ans == "fold");
                            Console.Clear();

                            if(ans == "double")
                            {
                                bj.SaveCredits(credits -= bet);
                                bj.Hit(ans);
                            }
                            else if(ans == "hit")
                            {
                                fHit = true;
                                bj.Hit(ans);
                            }
                        }while(ans != "double" && ans != "stand" && ans != "fold" && bj.valueP != 21);

                        credits += bj.Result(bet, ans);
                    }
                }
                else
                {
                    if(bj.valueP == 21 && bj.valueD != 21)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\nBLACKJACK!");
                        Console.ResetColor();
                        credits += (bet * 2) + (bet / 2); // pays 3 to 2.
                    }
                    else if(bj.valueP == bj.valueD)
                    {
                        bj.Push();
                        credits += bet;
                    }
                    else
                        bj.Lose();
                }
            }
            else
                throw new Exception("Out Of Credits!");

            return credits;
        }

        static void ExcText(Exception e) // exceptions text.
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{e.Message}");
            Console.ResetColor();
        }
    }
}
