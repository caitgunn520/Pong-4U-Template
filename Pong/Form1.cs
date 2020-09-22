/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int ballSpeed = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 40;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //set Width and Height of ball
            ball.Width = 20;
            ball.Height = 20;

            //set starting X position for ball to middle of screen, (use this.Width and ball.Width)
            ball.X = this.Width / 2 - ball.Width / 2;

            //set starting Y position for ball to middle of screen, (use this.Height and ball.Height)
            ball.Y = this.Height / 2 - ball.Height / 2;

            //resets ball speed
            ballSpeed = 4;

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            if (ballMoveRight == true)
            {
                ball.X = ball.X + ballSpeed;
            }
            else
            {
                ball.X = ball.X - ballSpeed;
            }

            if (ballMoveDown == true)
            {
                ball.Y += ballSpeed;
            }
            else
            {
                ball.Y -= ballSpeed;
            }

            #endregion

            #region update paddle positions

            if (aKeyDown == true && p1.Y > 0)
            {
                p1.Y -= PADDLE_SPEED;
            }

            if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y += PADDLE_SPEED;
            }

            if (jKeyDown == true && p2.Y > 0)
            {
                p2.Y -= PADDLE_SPEED;
            }

            if (mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y += PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                ballMoveDown = true;
                collisionSound.Play();
            }
            else if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if (p1.IntersectsWith(ball) || p2.IntersectsWith(ball))
            {
                collisionSound.Play();
                ballSpeed += 1;
                
                if (p1.IntersectsWith(ball))
                {
                    ballMoveRight = true;
                }  
                else
                {
                    ballMoveRight = false;
                }
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                player2Score += 1;
                scoreSound.Play();

                if (player2Score >= gameWinScore)
                {
                    GameOver("Player 2 Wins");
                }
                else
                {
                    ballMoveRight = true;
                    SetParameters();
                }
            }

            if (ball.X > this.Width - ball.Width)
            {
                player1Score += 1;
                scoreSound.Play();

                if (player1Score >= gameWinScore)
                {
                    GameOver("Player 1 Wins");
                }
                else
                {
                    ballMoveRight = false;
                    SetParameters();
                }
            }

            #endregion
            
            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;

            gameUpdateLoop.Stop();
            startLabel.Visible = true;
            startLabel.Text = winner;
            this.Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Do you want to play again? (Y/N)";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw paddles using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, p1);
            e.Graphics.FillRectangle(whiteBrush, p2);

            //draw ball using FillRectangle
            e.Graphics.FillRectangle(redBrush, ball);

            e.Graphics.DrawString("P1 Score " + player1Score, drawFont, whiteBrush, 10, 10);
            e.Graphics.DrawString("P2 Score " + player2Score, drawFont, whiteBrush, 500, 10);
        }

    }
}
