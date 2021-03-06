﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

/*После 50 очков меняется текстура монстров и их скорость увеличивается
  Подобрав коробку выпадает одно из трех действий: +Скорость (на время), Монстры останавливаются (на время), Монстры становятся такими же как после 50 монет*/

namespace Pacman
{
    public class RandomBox
    {
        public readonly PictureBox boxPic;
        private readonly string _path;

        public RandomBox()
        {
            _path = AppDomain.CurrentDomain.BaseDirectory;
            boxPic = boxPic = CreatePictureBox();
        }

        private PictureBox CreatePictureBox()
        {
            return new PictureBox() {
                Image = Image.FromFile(_path + "\\Sprites\\box.gif"),
                Name = "box",
                Size = new Size(50, 50),
                Location = new Point(350, 650),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }

        public void Roulete()
        {
            boxPic.Location = new Point(-50, -50);
            var random = new Random().Next(3);
            BoxChanged(random);
        }

        public event Action<int> BoxChanged;
    }
    
    public class Enemy
    {
        private readonly string _path;
        public readonly PictureBox EnemyPicYellow;
        public readonly PictureBox EnemyPicRed;

        public Enemy()
        {
            _path = AppDomain.CurrentDomain.BaseDirectory;
            EnemyPicYellow = CreatePictureBox("enemyYellow.png", 650 , 50, "enemyYellow");
            EnemyPicRed = CreatePictureBox("enemyRed.png", 50, 850, "enemyRed");
        }

        private PictureBox CreatePictureBox(string picture, int x, int y, string name)
        {
            return new PictureBox() {
                Image = Image.FromFile(_path + "\\Sprites\\" + picture),
                Name = name,
                Size = new Size(50, 50),
                Location = new Point(x, y),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }

        public static void Moving(Gamer gamer, Dictionary<Point, PictureBox> dic, PictureBox enemy)
        {
            var enemyX = enemy.Location.X;
            var enemyY = enemy.Location.Y;
            var playerX = gamer.PlayerPic.Location.X;
            var playerY = gamer.PlayerPic.Location.Y;
            if (playerX > enemyX && gamer.CanMove(dic, enemyX + 50, enemyY))
                enemy.Left += 50;
            else if (playerX < enemyX && gamer.CanMove(dic,enemyX - 50, enemyY))
                enemy.Left -= 50;
            else if (playerY > enemyY && gamer.CanMove(dic, enemyX, enemyY + 50))
                enemy.Top += 50;
            else if (playerY < enemyY && gamer.CanMove(dic, enemyX, enemyY - 50))
                enemy.Top -= 50;
        }

        public void TransformEnemy()
        {
            EnemyPicRed.Image = Image.FromFile(_path + "\\Sprites\\enemyBigRed.gif");
            EnemyPicYellow.Image = Image.FromFile(_path + "\\Sprites\\enemyBigRed.gif");
        }
    }
    
    public class Gamer
    {
        public readonly PictureBox PlayerPic;
        private readonly string _path;

        public Gamer()
        {
            _path = AppDomain.CurrentDomain.BaseDirectory;
            PlayerPic = CreatePictureBox();
        }

        private PictureBox CreatePictureBox()
        {
            return new PictureBox() {
                Image = Image.FromFile(_path + "\\Sprites\\playerRightGIF.gif"),
                Name = "player",
                Size = new Size(50, 50),
                Location = new Point(50, 50),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }

        public void AutoMoving(Dictionary<Point, PictureBox> dic, string direction, PictureBox enemyRed, PictureBox enemyYellow, RandomBox box)
        {
            var playerX = PlayerPic.Location.X;
            var playerY = PlayerPic.Location.Y;
            if (PlayerPic.Location == enemyRed.Location)
                Application.Exit();
            if (PlayerPic.Location == enemyYellow.Location)
                Application.Exit();
            if (PlayerPic.Location == box.boxPic.Location)
                box.Roulete();
            switch (direction)
            {
                case "Right" when playerX + 50 == 750 && playerY == 400:
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Location = new Point(0, 400);
                    PlayerChanged?.Invoke("playerRightGIF.gif");
                    break;
                case "Right" when CanMove(dic, playerX + 50, playerY):
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Left += 50;
                    break;
                case "Left" when playerX - 50 == -50 && playerY == 400:
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Location = new Point(700, 400);
                    PlayerChanged?.Invoke("playerLeftGIF.gif");
                    break;
                case "Left" when CanMove(dic, playerX - 50, playerY):
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Left -= 50;
                    break;
                case "Up" when CanMove(dic, playerX, playerY - 50):
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Top -= 50;
                    break;
                case "Down" when CanMove(dic, playerX, playerY + 50):
                    CoinChanged?.Invoke(dic, playerX, playerY);
                    PlayerPic.Top += 50;
                    break;
            }
        }

        public bool CanMove(Dictionary<Point, PictureBox> dic, int x, int y)
        {
            return dic.ContainsKey(new Point(x, y));
        }
        
        public string DirectionPlayer(Field field, Gamer gamer, KeyEventArgs args, string direction)
        {
            var dir = direction;
            var playerX = gamer.PlayerPic.Location.X;
            var playerY = gamer.PlayerPic.Location.Y;
            switch (args.KeyCode.ToString())
            {
                case "D" when CanMove(field.Dic, playerX + 50, playerY):
                    dir = "Right";
                    PlayerChanged?.Invoke("playerRightGIF.gif");
                    break;
                case "A" when CanMove(field.Dic, playerX - 50, playerY):
                    dir = "Left";
                    PlayerChanged?.Invoke("playerLeftGIF.gif");
                    break;
                case "W" when CanMove(field.Dic, playerX, playerY - 50):
                    dir = "Up";
                    PlayerChanged?.Invoke("playerUpGIF.gif");
                    break;
                case "S" when CanMove(field.Dic, playerX, playerY + 50):
                    dir = "Down";
                    PlayerChanged?.Invoke("playerDownGIF.gif");
                    break;
            }

            return dir;
        }
        
        public event Action<string> PlayerChanged;
        public event Action<Dictionary<Point, PictureBox>, int, int> CoinChanged;
    }
    public class Field
    {
        private readonly string _path;
        public const int Width = 15;
        public const int Height = 19;
        public PictureBox[,] Coins;
        public Dictionary<Point, PictureBox> Dic;

        public Field()
        {
            _path  = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void FillField()
        {
            Dic = new Dictionary<Point, PictureBox>();
            Coins = new PictureBox[Width,Height]; 
            var str = new[]
            {
                "XXXXXXXXXXXXXXX",
                "XSSSSSSXSSSSSSX",
                "XSXSXXSXSXXSXSX",
                "XSSSSSSSSSSSSSX",
                "XSXSXSXXXSXSXSX",
                "XSSSXSSXSSXSSSX",
                "XXXSXXSXSXXSXXX",
                "XXXSXSSSSSXSXXX",
                "SSSSSSXXXSSSSSS",
                "XXXSXSSSSSXSXXX",
                "XXXSXSXXXSXSXXX",
                "XSSSSSSXSSSSSSX",
                "XSXSXXSXSXXSXSX",
                "XSSSSSSSSSSSSSX",
                "XXXSXSXXXSXSXXX",
                "XSSSXSSXSSXSSSX",
                "XSXXXXSXSXXXXSX",
                "XSSSSSSSSSSSSSX",
                "XXXXXXXXXXXXXXX",
            };
            var pointX = -50;
            for (var x = 0; x < Width; x++)
            {
                pointX += 50;
                var pointY = 0;
                for (var y = 0; y < Height; y++)
                {
                    switch (str[y][x])
                    {
                        case 'S':
                        {
                            var coin = new PictureBox()
                            {
                                Image = Image.FromFile(_path + "\\Sprites\\point.png"),
                                Name = "go",
                                Size = new Size(50, 50),
                                Location = new Point(pointX, pointY),
                                SizeMode = PictureBoxSizeMode.StretchImage
                            };
                            Coins[x, y] = coin;
                            Dic[new Point(pointX, pointY)] = coin;
                            break;
                        }
                    }
                    pointY += 50;
                }
            }
        }
    }

    public class MyForm : Form
    {
        private MyForm(Field field, Gamer gamer, Enemy enemy, RandomBox box)
        {
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            var countCoins = 0;
            var timerEnemy = new Timer();
            var timerPlayer = new Timer();
            var timerCoin = new Timer();
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var direction = "";
            var pointCount = new Label()
            {
                ForeColor = Color.Red,
                Location = new Point(355,415),
                Size = new Size(42,20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pointCount.Font = new Font(pointCount.Font.FontFamily, 15.0f, pointCount.Font.Style);
            MakeMusic(path);
            Controls.Add(pointCount);
            Controls.Add(enemy.EnemyPicYellow);
            Controls.Add(enemy.EnemyPicRed);
            Controls.Add(gamer.PlayerPic);
            Controls.Add(box.boxPic);
            field.FillField();
            CreateField(field);
            timerEnemy.Start();
            timerEnemy.Interval = 450;
            timerPlayer.Start();
            timerPlayer.Interval = 200;
            timerCoin.Start();
            timerCoin.Interval = 300;
            this.KeyDown += (sender, args) =>
            {
                direction = gamer.DirectionPlayer(field, gamer, args, direction);
            };
            timerCoin.Tick += (sender, args) =>
            {
                if (countCoins != 50) return;
                enemy.TransformEnemy();
                timerEnemy.Interval = 350;
                timerCoin.Stop();
            };
            timerPlayer.Tick += (sender, args) =>
            {
                gamer.AutoMoving(field.Dic, direction, enemy.EnemyPicRed, enemy.EnemyPicYellow, box);
                pointCount.Text = countCoins.ToString();
            };
            timerEnemy.Tick += (sender, args) =>
            {
                Enemy.Moving(gamer, field.Dic, enemy.EnemyPicYellow);
                Enemy.Moving(gamer, field.Dic, enemy.EnemyPicRed);
            };
            gamer.PlayerChanged += (sender) =>
            {
                gamer.PlayerPic.Image = Image.FromFile(path + "\\Sprites\\" + sender);
            };
            gamer.CoinChanged += (dic, playerX, playerY) =>
            {
                if (!dic[new Point(playerX, playerY)].Visible) return;
                dic[new Point(playerX, playerY)].Hide();
                countCoins++;
            };
            box.BoxChanged += (random) =>
            {
                var label = new Label()
                {
                    Location = new Point(0, 0),
                    ForeColor = Color.FromArgb(255, Color.Aqua),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(ClientSize.Width, 30)
                };
                label.Font = new Font(pointCount.Font.FontFamily, 15.0f, pointCount.Font.Style);
                var timer = new Timer();
                var flag = true;
                switch (random)
                {
                    case 0:
                        label.Text = "STAND";
                        Controls.Add(label);
                        timer.Start();
                        timer.Interval = 1;
                        timer.Tick += (sender, args) =>
                        {
                            if (flag)
                            {
                                timer.Interval = 5000;
                                timerEnemy.Stop();
                                flag = false;
                            }
                            else
                            {
                                timerEnemy.Start();
                                timer.Stop();
                                label.Hide();
                            }
                        };
                        break;
                    case 1:
                        label.Text = "SPEED";
                        Controls.Add(label);
                        SwitchBox(timer, timerPlayer, null, label);
                        break;
                    case 2:
                        label.Text = "UPGRADE";
                        Controls.Add(label);
                        SwitchBox(timer, null, enemy, label);
                        break;
                }
            };
        }

        private void SwitchBox(Timer timer, Timer timerPlayer, Enemy enemy, Label label)
        {
            var flag = true;
            timer.Start();
            timer.Interval = 1;
            timer.Tick += (sender, args) =>
            {
                if (flag)
                {
                    timer.Interval = 5000;
                    enemy?.TransformEnemy();
                    if (timerPlayer != null)
                        timerPlayer.Interval = 100;
                    flag = false;
                }
                else
                {
                    if (timerPlayer != null)
                        timerPlayer.Interval = 200;
                    timer.Stop();
                    label.Hide();
                }
            };
        }
        
        private static void MakeMusic(string path)
        {
            var wmp = new WMPLib.WindowsMediaPlayer();
            wmp.settings.volume = 30;
            wmp.URL = path + "\\Sounds\\background.m4a";
            wmp.controls.play();
        }

        public static void Main()
        {
            var path = Directory.GetCurrentDirectory();
            var field = new Field();
            var gamer = new Gamer();
            var enemy = new Enemy();
            var box = new RandomBox();
            Application.Run(new MyForm(field, gamer, enemy, box) {ClientSize = new Size(750, 950),
                BackColor = Color.Black,
                BackgroundImage = Image.FromFile(path + "\\Sprites\\background.png")
            });
        }

        private void CreateField(Field field)
        {
            for (var x = 0; x < Field.Width; x++)
            for (var y = 0; y < Field.Height; y++)
            {
                Controls.Add(field.Coins[x,y]);
            }
        }
    }
}