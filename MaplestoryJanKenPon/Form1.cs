using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MaplestoryJanKenPon
{
    public partial class Form1 : Form
    {
        const double MaxRound = 9;
        double RoundCount = 0;
        double JanCount = 0;
        double KenCount = 0;
        double PonCount = 0;
        string LastBtn = "";
        const string AnswerRateSame = "現在出什麼勝率都一樣1/3";
        string AnswerTips1 = "你出\"{0}\"勝率最高";
        string AnswerTips2 = "你出\"{0}\"和\"{1}\"勝率最高";

        public Form1()
        {
            InitializeComponent();

            Lazy.Text = AnswerRateSame;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            switch (LastBtn)
            {
                case "J":
                    JanCount--;
                    Check();
                    break;
                case "K":
                    KenCount--;
                    Check();
                    break;
                case "P":
                    PonCount--;
                    Check();
                    break;
                case "R":


                    break;
                default:
                    break;
            }
            BtnBack.Enabled = false;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            LastBtn = "R";
            JanCount = 0;
            KenCount = 0;
            PonCount = 0;
            RoundCount = 0;
            BtnBack.Enabled = false;
            Check();
        }

        private void BtnJan_Click(object sender, EventArgs e)
        {
            LastBtn = "J";
            JanCount++;
            BtnBack.Enabled = true;
            Check();
        }

        private void BtnKen_Click(object sender, EventArgs e)
        {
            LastBtn = "K";
            KenCount++;
            BtnBack.Enabled = true;
            Check();
        }

        private void BtnPon_Click(object sender, EventArgs e)
        {
            LastBtn = "P";
            PonCount++;
            BtnBack.Enabled = true;
            Check();
        }

        void Check()
        {
            CountJan.Text = JanCount.ToString();
            CountKen.Text = KenCount.ToString();
            CountPon.Text = PonCount.ToString();
            RoundCount = JanCount + KenCount + PonCount;

            if (RoundCount == MaxRound)
            {
                BtnJan.Enabled = false;
                BtnKen.Enabled = false;
                BtnPon.Enabled = false;

                TipJan.Text = TipKen.Text = TipPon.Text = "0%";

                Lazy.Text = "此回結束囉";

                return;
            }
            else
            {
                BtnJan.Enabled = true;
                BtnKen.Enabled = true;
                BtnPon.Enabled = true;
            }

            BtnJan.Enabled = JanCount != 3;
            BtnKen.Enabled = KenCount != 3;
            BtnPon.Enabled = PonCount != 3;

            double jRate = (3 - JanCount) / (MaxRound - RoundCount);
            double kRate = (3 - KenCount) / (MaxRound - RoundCount);
            double pRate = (3 - PonCount) / (MaxRound - RoundCount);

            TipJan.Text = Math.Round(jRate * 100).ToString() + "%";
            TipKen.Text = Math.Round(kRate * 100).ToString() + "%";
            TipPon.Text = Math.Round(pRate * 100).ToString() + "%";

            Lazy.Text = Compare(jRate, kRate, pRate);
        }

        string Compare(double j, double k, double p)
        {
            if (j == k && k == p)
            {
                return AnswerRateSame;
            }

            if (j > k && j > p)
            {
                return string.Format(AnswerTips1, "石頭");
            }

            if (k > j && k > p)
            {
                return string.Format(AnswerTips1, "布");
            }

            if (p > j && p > k)
            {
                return string.Format(AnswerTips1, "剪刀");
            }

            if (j == k)
            {
                return string.Format(AnswerTips2, "石頭", "布");
            }

            if (k == p)
            {
                return string.Format(AnswerTips2, "布", "剪刀");
            }

            if (j == p)
            {
                return string.Format(AnswerTips2, "石頭", "剪刀");
            }

            return "";
        }
    }
}
