using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace The_Black_of_Jack
{
    internal class Hand
    {
        
        public List<string> CurrentCards { get; private set; }
        public List<PictureBox> FullCards { get; private set; }
        public string Name{ get; private set; }
        public Char Who{ get;  set; }
        public Boolean Double{ get;  set; }
        public Boolean Split{ get;   set; }//original hand
        public Boolean TheSplited{ get;   set; }//new hand
        public Label LblCounter { get; set; }



        public Hand(string name)
        {
            CurrentCards = new List<string>();
            FullCards = new List<PictureBox>();
            Who = 'E';
            Name = name;
        }

        public void AddCard(string card)
        {
            CurrentCards.Add(card);
        }
        public void Reset()
        {
            CurrentCards.Clear();
            FullCards.Clear();
            Double = false;
            Split = false;
        }

        //public Hand SpitIt(Hand hand)
        //{
        //    Hand Splited = new Hand();
        //    Splited.AddCard(CurrentCards[1]);
        //    Splited.FullCards.Add(FullCards[1]);
        //    Splited.Who = Who;
        //    Splited.TheSplited = true;
        //    //Splited.Split = true;
        //    CurrentCards.Remove(CurrentCards[1]);
        //    FullCards.Remove(FullCards[1]);
        //    Split = true;
        //    return Splited;
        //}

        public int GetTotalValue()
        {
            int totalValue = 0;
            int aceCount = 0;
            string cardValue;
            foreach (var card in CurrentCards)
            {
                cardValue = card.Split('_')[1];

                if (cardValue == "J" || cardValue == "Q" || cardValue == "K")
                    totalValue += 10;
                else if (cardValue == "Ace")
                {
                    aceCount++;
                    totalValue += 11;
                }
                else
                {
                    totalValue += int.Parse(cardValue);
                }
            }

            while (totalValue > 21 && aceCount > 0)
            {
                totalValue -= 10;
                aceCount--;
            }

            return totalValue;
        }
        public string ShowHidden()
        {
            return CurrentCards[1];
        }
        
    }
}
