using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_24___Tic_Tac_Toe_Game__.Properties;

namespace Project_24___Tic_Tac_Toe_Game__
{
    public partial class frmGame : Form
    {
        public frmGame()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Color White = Color.FromArgb(255, 255, 255, 255);

            Pen WhitePen = new Pen(White);

            WhitePen.Width = 13;
            WhitePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            WhitePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            e.Graphics.DrawLine(WhitePen, 615, 160, 615, 615);
            e.Graphics.DrawLine(WhitePen, 845, 160, 845, 615);

            e.Graphics.DrawLine(WhitePen, 415, 310, 1050, 310);
            e.Graphics.DrawLine(WhitePen, 415, 465, 1050, 465);
        }

        enum enPlayerTurn { Player1, Player2 }

        enum enWinner { Player1, Player2, Draw, GameInProgress }

        struct stGameStatus
        {
            public enWinner Winner;
            public byte PlayCount;
            public bool GameOver;
        }

        enPlayerTurn PlayerTurn = enPlayerTurn.Player1;

        stGameStatus GameStatus;

        private void DisableButtons()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
        }

        private void ResetButton(Button btn)
        {
            btn.Enabled = true;
            btn.Image = Resources.QuestionMark;
            btn.Tag = "?";
            btn.BackColor = Color.Transparent;
        }

        private void RestartGame()
        {
            ResetButton(button1);
            ResetButton(button2);
            ResetButton(button3);
            ResetButton(button4);
            ResetButton(button5);
            ResetButton(button6);
            ResetButton(button7);
            ResetButton(button8);
            ResetButton(button9);

            lblTurn.Text = "Player 1";
            lblWinner.Text = "In Progress";

            GameStatus.Winner = enWinner.GameInProgress;
            GameStatus.PlayCount = 0;
            GameStatus.GameOver = false;

            PlayerTurn = enPlayerTurn.Player1;
        }

        private void EndGame()
        {
            DisableButtons();

            GameStatus.GameOver = true;

            lblTurn.Text = "Game Over";

            switch (GameStatus.Winner)
            {
                case enWinner.Player1:

                    lblWinner.Text = "Player 1";
                    break;

                case enWinner.Player2:

                    lblWinner.Text = "Player 2";
                    break;
                
                default:

                    lblWinner.Text = "Draw";
                    break;
            }

            MessageBox.Show("Game Over !", "Game Over !", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool CheckValues(Button btn1, Button btn2, Button btn3)
        {
            if (btn1.Tag.ToString() != "?" && btn1.Tag.ToString() == btn2.Tag.ToString() && btn1.Tag.ToString() == btn3.Tag.ToString())
            {
                btn1.BackColor = Color.GreenYellow;
                btn2.BackColor = Color.GreenYellow;
                btn3.BackColor = Color.GreenYellow;

                if (btn1.Tag.ToString() == "X")
                {
                    GameStatus.Winner = enWinner.Player1;
                }
                else
                {
                    GameStatus.Winner = enWinner.Player2;
                }

                EndGame();
                return true;
            }

            GameStatus.GameOver = false;
            return false;
        }

        private void CheckWinner()
        {
            if (CheckValues(button1, button2, button3))
                return;

            if (CheckValues(button4, button5, button6))
                return;

            if (CheckValues(button7, button8, button9))
                return;

            if (CheckValues(button1, button4, button7))
                return;

            if (CheckValues(button2, button5, button8))
                return;

            if (CheckValues(button3, button6, button9))
                return;

            if (CheckValues(button1, button5, button9))
                return;

            if (CheckValues(button3, button5, button7))
                return;
        }

        private void ChangeImage(Button btn)
        {
            if (btn.Tag.ToString() == "?")
            {
                switch (PlayerTurn)
                {
                    case enPlayerTurn.Player1:

                        btn.Image = Resources.X;
                        btn.Tag = "X";
                        PlayerTurn = enPlayerTurn.Player2;
                        lblTurn.Text = "Player 2";
                       
                        break;

                    case enPlayerTurn.Player2:

                        btn.Image = Resources.O;
                        btn.Tag = "O";
                        PlayerTurn = enPlayerTurn.Player1;
                        lblTurn.Text = "Player 1";
                        
                        break;
                }

                GameStatus.PlayCount++;
                CheckWinner();
            }
            else
            {
                MessageBox.Show("Wrong Choice !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (GameStatus.PlayCount == 9 && !GameStatus.GameOver)
            {
                GameStatus.Winner = enWinner.Draw;
                EndGame();
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            ChangeImage((Button)sender);
        }

        private void btnRestartGame_Click(object sender, EventArgs e)
        {
            RestartGame();
        }
    }
}
