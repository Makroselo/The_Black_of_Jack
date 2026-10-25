using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace The_Black_of_Jack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void LBtnHow_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "How to Play Blackjack?\n\n" +
            "1. Card Values:\n" +
            "- Ace: 1 or 11 points.\n" +
            "- Face cards (J, Q, K): 10 points.\n" +
            "- Numbered cards: Their face value.\n\n" +
            "2. Game Start:\n" +
            "- Each player and the dealer are dealt two cards.\n" +
            "- Players' cards are face up; one of the dealer's cards is face down.\n\n" +
            "3. Actions:\n" +
            "- Hit: Take another card.\n" +
            "- Stand: Keep your current hand.\n" +
            "- Double: Double your bet and take one more card.\n" +
            "- Split: Split two cards of the same value into two hands.\n\n" +
            "Goal: Reach 21 or get closer to it than the dealer without exceeding it!",
            "Blackjack Guide",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );

        }

        int DecksInGame = 2;//52*DecksInGame
        int DeckCount = 8;//customize
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.txt");
        void SDI(int nv)//Selected Deck Index
        {
            File.WriteAllText(filePath, Convert.ToString(nv));
        }
        int SDI()
        {
            if (!File.Exists(filePath)) { SDI(0); return 0; }
            return Convert.ToInt32(File.ReadAllText(filePath));
        }

        private List<PictureBox> pictureBoxes = new List<PictureBox>();
        private void LBtnCustomize_Click(object sender, EventArgs e)
        {
            if (pictureBoxes.Count == 0)//zaten oyunda değil tuş basılmış mı ona bakıyor
            {
                for (int i = 0; i < DeckCount; i++)
                {
                    PictureBox pictureBox = GenerateItem(i + "_back", 20 + (120 * i), 450);
                    int cur = i;
                    pictureBox.Click += (s, ev) => PictureBox_Click(s, ev, cur);
                }
                pictureBoxes[SDI()].Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(SDI() + "s_K");
            }
            else 
            {
                Clear();
            }
            
        }
        private void PictureBox_Click(object s, EventArgs ev, int i)
        {
            int j = 0;
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                pictureBox.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(j++ + "_back");

            }
            PictureBox clickedPictureBox = s as PictureBox;
            clickedPictureBox.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(i + "s_K");
            SDI(i);
        }

        private PictureBox GenerateItem(string imageName, int x, int y, int w= 100, int h = 147)
        {
            
            PictureBox pictureBox = new PictureBox();
            pictureBox.Location = new System.Drawing.Point(x, y);
            //pictureBox.Size = new System.Drawing.Size(100, 147);
            pictureBox.Size = new System.Drawing.Size(w, h);
            pictureBox.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(imageName);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.BackColor = Color.Transparent;
            this.Controls.Add(pictureBox);
            pictureBoxes.Add(pictureBox);
            pictureBox.BringToFront();
            return pictureBox;
        }
        //void LobiState(bool state)
        //{
        //    BtnStart.Visible = state;
        //    BtnCustom.Visible = state;
        //    BtnHow.Visible = state;
        //    BtnBack.Visible = !state;
        //    //GBtnBetUp.Visible = !state;
        //    //BtnBetDown.Visible = false;
        //    BtnRoll.Visible = false;
        //    SeatsVisible(false);
        //    Clear();
        //    ClearActionButtons();
        //    ClearLbls();
        //}
        void Clear()
        {
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                this.Controls.Remove(pictureBox);
            }
            pictureBoxes.Clear();
        }
        List<Hand> AllHands= new List<Hand>();
        List<Hand> SplitedHands= new List<Hand>();
        private void Form1_Load(object sender, EventArgs e)
        {
            Dealer.Who = 'D';
            AllHands.Add(S1);
            AllHands.Add(S2);
            AllHands.Add(S3);
            AllHands.Add(Dealer);
            SplitedHands.Add(Sp1);
            SplitedHands.Add(Sp2);
            SplitedHands.Add(Sp3);
            Sp1.TheSplited = true;
            Sp2.TheSplited = true;
            Sp3.TheSplited = true;

        }
        void PlayTune(int trackCode)
        {
            SoundPlayer player;
            if (trackCode==0)
            {
                switch (new Random().Next(4))
                {
                    case 0:
                        player = new SoundPlayer(Properties.Resources.shuffle0);
                        break;
                    case 1:
                        player = new SoundPlayer(Properties.Resources.shuffle1);
                        break;
                    case 2:
                        player = new SoundPlayer(Properties.Resources.shuffle2);
                        break;
                    default:
                        player = new SoundPlayer(Properties.Resources.shuffle3);
                        break;
                }
            }
            else 
            {
                switch (new Random().Next(3))
                {
                    case 0:
                        player = new SoundPlayer(Properties.Resources.deal0);
                        break;
                    case 1:
                        player = new SoundPlayer(Properties.Resources.deal1);
                        break;
                    default:
                        player = new SoundPlayer(Properties.Resources.deal2);
                        break;
                }
            }
            player.Play();
            
        }



        List<string> deck = new List<string>();
        void SetTheDeck()
        {
            _turn = 0;
            string[] suits = { "s", "h", "d", "c" };
            string[] values = { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            deck.Clear();
            
            for (int i = 0; i < DecksInGame; i++)//how many decks in play
            {
                foreach (string s in suits)//type of cards
                {
                    foreach(string v in values)//values
                    {
                        deck.Add(SDI()+s +"_"+ v);
                    }
                }
            }
        }
        
        string RandCard()
        {
            int value = new Random().Next(deck.Count);
            string s;
            try{
                s = deck[value];
                deck.RemoveAt(value);
            }
            catch (Exception){s = "0_back"; }//Default
            return s;
        }
        private void LBtnStart_Click(object sender, EventArgs e)
        {
            if (money==0)
            {
                MessageBox.Show("Be more careful next time, and don't forget this favor I did for you!", "Your balance has been set to 100$", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            ThePhase(1);
            //LobiState(false);
            //BtnRoll.Visible = true;
            //SeatsVisible(true);
            
        }

        public int stake
        {
            get
            {
                return Convert.ToInt32(LblBet.Text.Replace("$", ""));
            }
            set
            {
                LblBet.Text = Convert.ToString(value) + '$';
                if (stake == 0)
                    BtnBetDown.Visible = false;
            }
        }
        public int money
        {
            get
            {
                return Convert.ToInt32(blBallance.Text.Replace("$", ""));
            }
            set
            {
                blBallance.Text = Convert.ToString(value) + '$';
                if (money < 10)
                    BtnBetUp.Visible = false;
            }
        }

        private void GBtnBack_Click(object sender, EventArgs e)
        {
            if (pictureBoxes.Count==0)
                money = money + stake;
            stake = 0;
            ThePhase(0);

        }

        private void GBtnBetUp_Click(object sender, EventArgs e)
        {
            if (money>=pCount*10)
            {
                BtnBetDown.Visible = true;
                money -= pCount*10;
                stake += pCount*10;
                if (money == 0)
                    BtnBetUp.Visible = false;
            }
            else
                MessageBox.Show("You don't have enough money!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnBetDown_Click(object sender, EventArgs e)
        {
            BtnBetUp.Visible = true;
            money += pCount * 10;
            stake -= pCount * 10;
            if(stake==0)
                BtnBetDown.Visible = false;
        }
        private void GLblBet_TextChanged(object sender, EventArgs e)
        {
            LblBet.Visible = Convert.ToInt32(LblBet.Text.Replace("$", "")) != 0;
        }
        Hand Dealer = new Hand("Dealer");
        Hand S1 = new Hand("S1");
        Hand S2 = new Hand("S2");
        Hand S3 = new Hand("S3");

        Hand Sp1 = new Hand("Sp1");
        Hand Sp2 = new Hand("Sp2");
        Hand Sp3 = new Hand("Sp3");
        PictureBox hidden;
        int spliteds;
        async Task Deal(Hand guy,int Place=0 ,bool? hide = null)
        {
            int number = guy.CurrentCards.Count;
            if (guy.Who == 'D')//Dealer
            {
                if (hide == true)
                {
                    string card = RandCard();
                    guy.AddCard(card);
                    hidden = GenerateItem(Convert.ToString(SDI()) + "_back", 540 - (number * 20), 10 + (number * 40));
                }
                else if (hide == false)
                {
                    GenerateItem(guy.ShowHidden(), 540 - (1 * 20), 10 + (1 * 40));
                    this.Controls.Remove(hidden);
                    CounLbl(number, Place);
                }
                else
                {
                    string card = RandCard();
                    guy.AddCard(card);
                    GenerateItem(card, 540 - (number * 20), 10 + (number * 40));
                    CounLbl(number, Place);
                }
            }
            else//not dealer
            {
                string card = RandCard();
                guy.AddCard(card);
                //guy.FullCards.Add(GenerateItem(card, 180 + ((Place) * 300) + (number * 20), 420 - (number * 40)));
                bool origin=true;
                int xAxis = 180 + (Place * 300) + (number * 20);  //standart x
                int yAxis = 420 - (number * 40);                  //standart button y
                if (guy.Split == true)      //goes to left
                    xAxis -= 70;
                if (guy.TheSplited == true)  //goes to right
                {
                    origin = false;
                    xAxis += 90;
                }
                guy.FullCards.Add(GenerateItem(card, xAxis, yAxis));

                CounLbl(number,Place,origin);

            }
            PlayTune(1);
            await WaitAsync();
        }

        private List<Label> CountLbls = new List<Label>();
        void CounLbl(int thCart,int Place,bool origin=true)
        {
            int xAxis= 290 + (Place * 300);
            Hand currenthnd;

            if (origin)
                currenthnd = AllHands[Place];
            else
            {
                currenthnd = SplitedHands[Place];
                xAxis += 90;
            }
            if (thCart == 0)
            {
                Label countLbl = new Label();
                countLbl.Name = "LblCount" + currenthnd.Name;
                countLbl.Text = Convert.ToString(currenthnd.GetTotalValue());
                countLbl.AutoSize = true;
                if (Place==3)//dealer
                    countLbl.Location = new Point(650, 130);
                else
                    countLbl.Location = new Point(xAxis, 540);
                countLbl.Font =LblNew.Font;
                CountLbls.Add(countLbl);
                currenthnd.LblCounter = countLbl;
                this.Controls.Add(countLbl);
                countLbl.BringToFront();
            }
            else
            {
                Label countLbl = currenthnd.LblCounter;
                countLbl.Text = Convert.ToString(currenthnd.GetTotalValue());
            }

        }
        void ClearLbls()
        {
            if(CountLbls.Count > 0)
            {
                foreach(Label lbl in CountLbls)
                {
                    this.Controls.Remove(lbl);
                }
                CountLbls.Clear();
            }
        }
        private async Task WaitAsync()
        {
            await Task.Delay(300);
        }
        void StatusCtrl(Hand hnd)
        {
            if (hnd.GetTotalValue() >= 21|| hnd.Double==true)
            {
                ClearActionButtons();
                if (hnd != Dealer)// not dealer
                {
                    if (hnd.GetTotalValue() > 21)
                    {
                        //Bust
                        if (hnd.Split)
                            MessageBox.Show("Seat number " + (AllHands.IndexOf(hnd) + 1) + " busted their first hand:(");
                        else if (hnd.TheSplited)
                            MessageBox.Show("Seat number " + (SplitedHands.IndexOf(hnd) + 1) + " busted their second hand:(");
                        else
                            MessageBox.Show("Seat number " + (AllHands.IndexOf(hnd) + 1) + " bust :(");
                        if (hnd.Split==true)
                            hnd.FullCards[hnd.FullCards.Count-1].Location= new Point(110 + ((turn) * 300), 420);
                        else if (hnd.TheSplited == true)
                            hnd.FullCards[hnd.FullCards.Count - 1].Location = new Point(270 + ((turn) * 300), 420);
                        else
                            hnd.FullCards[hnd.FullCards.Count - 1].Location = new Point(180 + ((turn) * 300), 420);
                        for (int i = 0; i < hnd.FullCards.Count - 1; i++)
                        {
                            this.Controls.Remove(hnd.FullCards[i]);
                        }
                        if (AllHands[turn].Split == true && !SplitsTurn)
                        {
                            AddActionButtons(true);
                            SplitsTurn = true;
                        }
                        else
                        {
                            AddActionButtons(true);
                            SplitsTurn = false;
                        }

                        if (!SplitsTurn)
                            turn++;
                        else
                            AddActionButtons();
                    }
                    else if (hnd.CurrentCards.Count==2)
                    {
                        if (hnd.Split)
                            MessageBox.Show("Seat number " + (AllHands.IndexOf(hnd) + 1) + " have blackJack on their first hand:)");
                        else if (hnd.TheSplited)
                            MessageBox.Show("Seat number " + (SplitedHands.IndexOf(hnd) + 1) + " have blackJack on their second hand:)");
                        else
                            MessageBox.Show("Seat number " + (AllHands.IndexOf(hnd) + 1) + " blackJack :)");
                    }
                    else
                    {
                        if (AllHands[turn].Split == true && !SplitsTurn)
                        {
                            AddActionButtons(true);
                            SplitsTurn = true;
                        }
                        else
                        {
                            AddActionButtons(true);
                            SplitsTurn = false;
                        }

                        if (!SplitsTurn)
                            turn++;
                        else
                            AddActionButtons();
                    }
                }
                else//dealer
                {
                    if (hnd.GetTotalValue() > 21)
                    {
                        //Bust
                        MessageBox.Show("Dealer bust :)");
                    }
                    else if(hnd.CurrentCards.Count == 2)
                    {
                        //BlackJack
                        MessageBox.Show("Dealer blackJack :/");
                    }
                }
            }
        }
        int bpp;
        private List<PictureBox> ActionButtons = new List<PictureBox>();
        void AddActionButtons(Boolean jstStnd=false)
        {
            if (jstStnd)
            {
                if (!bust(AllHands[turn]) && AllHands[turn].Double)
                {//only Doubles
                    AddButton(1,true);
                }
                else
                {//only stnd
                    AddButton(2, true);
                }
            }
            else
            {//full set
                AddButton(0).Click += (s, ev) => BtnHit_Click(s, ev);
                AddButton(1).Click += (s, ev) => BtnDouble_Click(s, ev);
                AddButton(2).Click += (s, ev) => BtnStand_Click(s, ev);
                if (AllHands[turn].Split == false && AllHands[turn].TheSplited == false)
                    AddButton(3).Click += (s, ev) => BtnSplit_Click(s, ev);
            }
        }

        PictureBox AddButton(int type, Boolean only=false)
        {
            PictureBox actionbutton = new PictureBox();
            int xAxis= 175+(turn * 300);            //standart hit x
            int yAxis = 570;

            if (SplitsTurn == true)
                xAxis += 90;
            else if (AllHands[turn].Split == true)       //goes to left
                xAxis -= 70 - 15;
            if (only)
            {
                xAxis += 45;
                if (AllHands[turn].Split == true || AllHands[turn].TheSplited == true)
                    xAxis -= 15;                    //already-1 button
            }
            else 
            { 
                xAxis += 30 * type;
            }

            switch (type)
            {
                case 0://Hit
                    actionbutton = GenerateItem("Hit", xAxis, yAxis, 25, 25);
                    ActionButtons.Add(actionbutton);
                    return actionbutton;
                case 1://double
                    actionbutton = GenerateItem("Double", xAxis, yAxis, 25, 25);
                    if (!only)
                        ActionButtons.Add(actionbutton);
                    return actionbutton;
                case 2://StanD
                    actionbutton = GenerateItem("Stand", xAxis, yAxis, 25, 25);
                    if (!only)
                        ActionButtons.Add(actionbutton);
                    return actionbutton;
                case 3://split
                    actionbutton = GenerateItem("Split", xAxis, yAxis, 25, 25);
                    ActionButtons.Add(actionbutton);
                    return actionbutton;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Type value must be between 0 and 3.");
            }
        }

        private async void BtnSplit_Click(object s, EventArgs ev)
        {
            if (money >= bpp)
            {
                money -= bpp;
                stake += bpp;
                PictureBox leftcard = AllHands[turn].FullCards[0];
                PictureBox rightcard = AllHands[turn].FullCards[1];
                SplitedHands[turn].AddCard(AllHands[turn].CurrentCards[1]);
                SplitedHands[turn].FullCards.Add(rightcard);
                AllHands[turn].CurrentCards.RemoveAt(1);
                AllHands[turn].FullCards.RemoveAt(1);
                leftcard.Location = new Point(leftcard.Location.X - 70, leftcard.Location.Y);
                rightcard.Location = new Point(rightcard.Location.X + 70, leftcard.Location.Y);
                AllHands[turn].Split = true;
                CountLbls[turn].Location = new Point(CountLbls[turn].Location.X - 70, CountLbls[turn].Location.Y);
                ClearActionButtons();
                CounLbl(0, turn, false);
                await Deal(AllHands[turn], turn);
                StatusCtrl(AllHands[turn]);
                await Deal(SplitedHands[turn], turn);
                StatusCtrl(SplitedHands[turn]);
                if (!bj(AllHands[turn]))
                    AddActionButtons();
                else
                    BtnStand_Click(s, ev);
            }
            else
                MessageBox.Show("You don't have enough money!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private async void BtnDouble_Click(object s, EventArgs ev)
        {
            if (money >= bpp)
            {
                money -= bpp;
                stake += bpp;
                if (SplitsTurn)
                {
                    SplitedHands[turn].Double = true;
                    await Deal(SplitedHands[turn], turn);
                    StatusCtrl(SplitedHands[turn]);
                }
                else
                {
                    AllHands[turn].Double = true;
                    await Deal(AllHands[turn], turn);
                    StatusCtrl(AllHands[turn]);
                }
            }
            else 
            {
                MessageBox.Show("You don't have enough money!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        

        void ClearActionButtons()
        {
            foreach (PictureBox actions in ActionButtons)
            {
                this.Controls.Remove(actions);
            }
            ActionButtons.Clear();
        }
        private async void BtnHit_Click(object s, EventArgs ev)
        {
            if (SplitsTurn)
            {
                await Deal(SplitedHands[turn], turn);
                StatusCtrl(SplitedHands[turn]);
            }
            else
            {
                await Deal(AllHands[turn], turn);
                StatusCtrl(AllHands[turn]);
            }
        }
        private void BtnStand_Click(object s, EventArgs ev)
        {
            ClearActionButtons();
            if (AllHands[turn].Split == true && !SplitsTurn)
            {
                AddActionButtons(true);
                SplitsTurn = true;
            }
            else
            {
                AddActionButtons(true);
                SplitsTurn = false;
            }
            if (!SplitsTurn)
                turn++;
            else
            {
                if (!bj(SplitedHands[turn]))
                    AddActionButtons();
                else
                    AddActionButtons(true);
            }
                
        }
        int _pCount;
        public int pCount
        {
            get{ return _pCount; }
            set
            {
                _pCount=value;
                if (pCount == 0)
                {
                    money = money + stake;
                    stake = 0;
                    if (stake != 0)
                    {
                        
                    }
                    BtnBetUp.Visible = false;
                }
                else { BtnBetUp.Visible = true; }
            }
        }
        
        private async void BtnRoll_Click(object sender, EventArgs e)
        {
            if (pCount > 0)
            {
                bpp = stake/pCount;
                SplitsTurn = false;
                ThePhase(2);
                //BtnBetDown.Visible = false;
                //BtnBetUp.Visible = false;
                //BtnRoll.Visible = false;
                //SeatsVisible(false);
                foreach (Hand handres in AllHands.Concat(SplitedHands))
                {
                    handres.Reset();
                }
                //Dealer.Reset();
                //S1.Reset();
                //S2.Reset();   
                //S3.Reset();
                spliteds = 0;
                SetTheDeck();
                PlayTune(0); 
                await WaitAsync();
                await WaitAsync();
                BtnBack.Visible = false;
                await forceDeal();
                GameManager();
            }
            else
            {
                MessageBox.Show("You need to sit to play!", "Sit Somewhere", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private int _turn = 0;
        public int turn
        {
            get { return _turn%10; }//0 _0_ 
            set
            {
                AddActionButtons(true);
                _turn = value + (_turn - _turn % 10);
                GameManager();
            }
        }
        bool SplitsTurn = false;
        void ThePhase(int State=0)
        {
            switch (State)
            {
                case 0://lobby phase
                    BtnStart.Visible = true;
                    BtnCustom.Visible = true;
                    BtnHow.Visible = true;
                    BtnBack.Visible = false;
                    BtnRoll.Visible = false;
                    BtnBetUp.Visible=false;
                    BtnBetDown.Visible = false;
                    SeatsVisible(false);
                    break;
                case 1://Limbo phase
                    BtnStart.Visible = false;
                    BtnCustom.Visible = false;
                    BtnHow.Visible = false;
                    BtnBack.Visible = true;
                    BtnRoll.Visible = true;
                    SeatsVisible(true);
                    if (pCount>0)
                    {
                        BtnBetUp.Visible = true;
                    }
                    break;
                case 2://Game phase
                    BtnBetDown.Visible = false;
                    BtnBetUp.Visible = false;
                    BtnRoll.Visible = false;
                    SeatsVisible(false);
                    break;
            }
            Clear();
            ClearActionButtons();
            ClearLbls();
        }

        async void GameManager()
        {
            BtnBack.Visible=false;
            Hand gameHand = AllHands[turn];
            if (gameHand.Who == 'P')
            {
                if (!bj(gameHand))
                    AddActionButtons();
                else
                    turn++;
            }
            else if (gameHand.Who == 'B')
            {
                int trn = turn;
                while (-10 * (gameHand.GetTotalValue() - 21) > new Random().Next(-1, 100))
                {
                    await Deal(gameHand, turn);
                    StatusCtrl(AllHands[turn]);
                }
                if (turn == trn) { turn++; }
            }
            else if (gameHand.Who == 'D')//dealer
            {
                await Deal(gameHand, turn, false);//kapalı kartı aç
                bool gtg = false;
                foreach (Hand fH in AllHands)
                {
                    if (!bust(fH)&&fH!=Dealer)
                    {
                        gtg = true;
                        break; 
                    }
                }
                if (gtg)
                {
                    while (gameHand.GetTotalValue() < 17)
                    {
                        await Deal(gameHand, turn);
                        StatusCtrl(AllHands[turn]);
                    }
                }
                else
                {
                    MessageBox.Show("Eveyone bust :D");
                }
                int plus = 0;
                for (int i = 0; i < 3; i++)
                {
                    Hand fH = AllHands[i];
                    Hand fsH = SplitedHands[i];
                    if (fH.Who != 'D' && fH.Who != 'B'&& fH.Who != 'E')
                    {
                        double jst = bpp;
                        if (fH.Double)
                            jst *= 2;
                        plus += Convert.ToInt32(jst * finalCheck(fH));
                    }
                    if (fH.Split==true)
                    {
                        double jsst = bpp;
                        if (fsH.Double)
                            jsst *= 2;
                        plus += Convert.ToInt32(jsst * finalCheck(fsH));
                    }
                }

                MessageBox.Show("You won "+ plus +"$");
                money += plus;
                stake = 0;
                ThePhase(1);

            }
            else { _turn++; GameManager(); }//E
            BtnBack.Visible = true;
        }
        double finalCheck(Hand hand)
        {
            if (bust(hand)||(!bust(Dealer)&& hand.GetTotalValue() < Dealer.GetTotalValue())|| !bj(hand) && bj(Dealer))
            {
                return 0;
            }
            else if (bj(hand)&&bj(Dealer)||hand.GetTotalValue()==Dealer.GetTotalValue())
            {
                return 1;
            }
            else if(bj(hand))
            {
                return 2.5;
            }
            return 2;

        }
        Boolean bj(Hand hand)
        {
            return hand.GetTotalValue() == 21 && hand.CurrentCards.Count == 2;
        }
        Boolean bust(Hand hand)
        {
            return hand.GetTotalValue() > 21;
        }


        async Task forceDeal()
        {
            for (int i = 0; i < 2; i++)
            {
                int j = 0;//Place
                foreach (Hand forceHand in AllHands)
                {
                    if (forceHand == Dealer&& i == 1)
                    {
                        await Deal(forceHand, 0, true);
                    }
                    else 
                    {
                        if (forceHand.Who != 'E')
                        {
                            await Deal(forceHand, j);
                            StatusCtrl(AllHands[j++]);
                        }                        
                        else
                            j++;
                    }
                }
            }
        }
        void SeatsVisible(bool visible)
        {
            for (int i = 1; i < 4; i++)
            {
                PictureBox seatBtn = this.Controls.Find("BtnSeat" + Convert.ToString(i)+"P", true).FirstOrDefault() as PictureBox;
                seatBtn.Visible = visible;
                //if (visible)
                //{
                //    seatBtn.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("ButtonAdd" + seatBtn.Name[8]);
                //}
            }
            for (int i = 1; i < 4; i++)
            {
                PictureBox seatBtn = this.Controls.Find("BtnSeat" + Convert.ToString(i) + "B", true).FirstOrDefault() as PictureBox;
                seatBtn.Visible = visible;
                //if (visible)
                //{
                //    seatBtn.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("ButtonAdd" + seatBtn.Name[8]);
                //}
            }
            //if (visible)
            //{
            //    foreach (Hand itemE in AllHands)
            //    {
            //        if (itemE.Who!='D')
            //        {
            //            itemE.Who = 'E';
            //        }
            //    }
            //    pCount = 0;
            //}

        }
        //private void Btneat_Clicks(object sender, EventArgs e)
        //{
        //    PictureBox clickedButton = sender as PictureBox;
        //    Hand hand = GetType().GetField("S" + clickedButton.Name[7], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Hand;
        //    if (clickedButton.Tag == "0")
        //    {
        //        char anti = 'P';
        //        if (clickedButton.Name[8] == 'P') { anti = 'B'; }
        //        hand.Who = clickedButton.Name[8];
        //        clickedButton.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("ButtonRemove" + clickedButton.Name[8]);
        //        clickedButton.Tag = "1";
        //        PictureBox otherBtn = this.Controls.Find("BtnSeat" + clickedButton.Name[7] + anti, true).FirstOrDefault() as PictureBox;
        //        if (hand.Who == 'P')
        //            pCount++;
        //        else if (otherBtn.Tag=="1")
        //            pCount--;
        //        otherBtn.Image= (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("ButtonAdd" + anti);
        //        otherBtn.Tag = "0";
        //    }
        //    else
        //    {
        //        if (hand.Who=='P')
        //            pCount--;
        //        hand.Who = 'E'; 
        //        clickedButton.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("ButtonAdd" + clickedButton.Name[8]);
        //        clickedButton.Tag = "0";
        //    }
            
        //}
        private void BtnSeat_Clicks(object sender, EventArgs e)
        {
            PictureBox clickedButton = sender as PictureBox;
            Hand hand = GetType().GetField("S" + clickedButton.Name[7], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Hand;
            PictureBox otherBtn;
            if (clickedButton.Name[8] == 'P')
                otherBtn = this.Controls.Find("BtnSeat" + clickedButton.Name[7] + 'B', true).FirstOrDefault() as PictureBox;
            else
                otherBtn = this.Controls.Find("BtnSeat" + clickedButton.Name[7] + 'P', true).FirstOrDefault() as PictureBox;

            switch (hand.Who)
            {
                case 'E':
                    if (clickedButton.Name[8] == 'P')// E --> P     P
                    {
                        if (moneyChck())
                        {
                            SetIt(clickedButton, true, 'P');
                            hand.Who = 'P';
                            if (pCount!=0)
                            {
                                money -= stake / pCount;
                                stake += stake / pCount;
                            }
                            pCount++;
                        }
                        else
                        {
                            MessageBox.Show("You don't have enough money!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else//                              E --> B     B
                    {
                        SetIt(clickedButton,true, 'B');
                        hand.Who = 'B';
                    }
                    break;
                case 'P':
                    if (clickedButton.Name[8] == 'P')// P --> E     P
                    {
                        SetIt(clickedButton,false, 'P');
                        hand.Who = 'E';
                        money += stake / pCount;
                        stake -= stake / pCount;
                        pCount--;
                    }
                    else//                              P --> B     B
                    {
                        SetIt(clickedButton,true,'B');
                        SetIt(otherBtn,false,'P');
                        hand.Who = 'B';
                        money += stake / pCount;
                        stake -= stake / pCount;
                        pCount--;
                    }
                    break;
                case 'B':
                    if (clickedButton.Name[8] == 'P')// B --> P     P
                    {
                        if (moneyChck())
                        {
                            SetIt(clickedButton, true, 'P');
                            SetIt(otherBtn, false, 'B');
                            hand.Who = 'P';
                            if (pCount != 0)
                            {
                                money -= stake / pCount;
                                stake += stake / pCount;
                            }
                            pCount++;
                        }
                        else
                        {
                            MessageBox.Show("You don't have enough money!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else//                              B --> E     B
                    {
                        SetIt(clickedButton, false, 'B');
                        hand.Who = 'E';
                    }
                    break;
                default:
                    MessageBox.Show("Hmm... I guess u found a bug.");
                    break;
            }
        }
        void SetIt(PictureBox Button, bool adding, char T)
        {
            string name;
            if (!adding)
                name = "ButtonAdd" + T;
            else 
                name = "ButtonRemove" + T;
            Button.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(name);

        }
        Boolean moneyChck()
        {
            if (stake != 0 && pCount != 0)
            {
                return money > stake / pCount;
            }
            else
                return true;
        }
        
    }
}
