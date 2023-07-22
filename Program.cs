using System;
using SFML.Learning;
using SFML.Window;

namespace Find_Couple
{
    class Program : Game
    {
        static string[] icons;
        static string BGM = LoadMusic("bg_music.wav");
        static string cardSwapS = LoadSound("card_swap.wav");

        static uint winH = 600;
        static uint winW = 800;
        struct Card
        {
            public int x;
            public int y;
            public int sizeX;
            public int sizeY;
            public int value;
            public int status;
        }

        static void Main()
        {
            InitWindow(winW, winH, "Window");
            LoadIcons();
            SetFont("arial.ttf");
            PlayMusic(BGM, 20);
            for (int difficulty = Menu(); difficulty != -1; difficulty = Menu())
            {
                int x = 3 + difficulty;
                int y = 2 + difficulty;
                Card[,] Cards = Initialize(difficulty, x, y);
                Game(Cards, x, y, difficulty);
            }
        }

        static void LoadIcons()
        {
            icons = new string[7];
            icons[6] = "";
            for (int i = 0; i < 6; i++)
            {
                icons[i] = LoadTexture($"Icon_{i + 1}.png");
            }
        }

        static int Menu()
        {
            // параметры кнопок меню
            int[,] buttonCoordinate = new int[4, 2] { { 200, 180 }, { 200, 270 }, { 200, 360 }, { 200, 480 } };
            int[] buttonSize = new int[2] { 400, 50 };

            int result = 0;
            while (result == 0)
            {
                DispatchEvents();
                if (GetMouseButtonDown(Mouse.Button.Left))
                {
                    result = ClickMenu(buttonCoordinate, buttonSize);
                }
                ClearWindow();
                FillMenu(buttonCoordinate, buttonSize);
                DisplayWindow();
                Delay(1);
            }
            return result;
        }
        static void FillMenu(int[,] Coord, int[] Size)
        {
            SetFillColor(255, 255, 255);
            FillRectangle(Coord[0, 0], Coord[0, 1], Size[0], Size[1]);
            FillRectangle(Coord[1, 0], Coord[1, 1], Size[0], Size[1]);
            FillRectangle(Coord[2, 0], Coord[2, 1], Size[0], Size[1]);
            FillRectangle(Coord[3, 0], Coord[3, 1], Size[0], Size[1]);
            DrawText(300, 50, "Find Couples", 36);
            SetFillColor(50, 50, 50);
            DrawText(355, 181, "Легко", 36);
            DrawText(305, 271, "Нормально", 36);
            DrawText(335, 361, "Сложно", 36);
            DrawText(345, 481, "Выход", 36);
        }

        static int ClickMenu(int[,] Coord, int[] Size)
        {
            int result = 0;
            if (MouseX >= Coord[0, 0] && MouseX <= Coord[0, 0] + Size[0] && MouseY >= Coord[0, 1] && MouseY <= Coord[0, 1] + Size[1]) result = 1;
            if (MouseX >= Coord[1, 0] && MouseX <= Coord[1, 0] + Size[0] && MouseY >= Coord[1, 1] && MouseY <= Coord[1, 1] + Size[1]) result = 2;
            if (MouseX >= Coord[2, 0] && MouseX <= Coord[2, 0] + Size[0] && MouseY >= Coord[2, 1] && MouseY <= Coord[2, 1] + Size[1]) result = 3;
            if (MouseX >= Coord[3, 0] && MouseX <= Coord[3, 0] + Size[0] && MouseY >= Coord[3, 1] && MouseY <= Coord[3, 1] + Size[1]) result = -1;
            return result;
        }

        static Card[,] Initialize(int difficulty, int x, int y)
        {
            Card[,] cards = new Card[x, y];
            for (int i = 0, count = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++, count++)
                {
                    if (count > 11) count = 0;
                    cards[i, j].x = 200 / (x * (difficulty + 1) / 2) + (i * 2 * 400) / x;
                    cards[i, j].y = 150 / (y * (difficulty + (difficulty + 2) / 2) / 2) + (j * 2 * 300) / y;
                    cards[i, j].sizeX = 100;
                    cards[i, j].sizeY = 100;
                    cards[i, j].status = 0;
                    cards[i, j].value = count / 2;
                }
            }
            Shuffle(cards, x, y);
            return cards;
        }

        static void Shuffle(Card[,] cards, int x, int y)
        {
            var rand = new Random();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    int k = rand.Next(x);
                    int l = rand.Next(y);
                    int temp = cards[i, j].value;
                    cards[i, j].value = cards[k, l].value;
                    cards[k, l].value = temp;
                }
            }
        }

        static void Game(Card[,] Cards, int x, int y, int difficulty)
        {
            int interactive = 1;
            int pause = 0;
            int timer = 0;
            while(true)
            {
                DispatchEvents();
                if (interactive == 1)
                {
                    if (GetMouseButtonDown(Mouse.Button.Left))
                    {
                        ClickGame(Cards, x, y);
                    }
                    int status;
                    if ((status = StatusCheck(Cards, x, y)) == 1)
                    {
                        interactive = 0;
                    }
                    else if (status == 2)
                    {
                        break;
                    }
                }
                else
                {
                    pause++;
                    if (pause >= 60)
                    {
                        interactive = 1;
                        pause = 0;
                        StatusChange(Cards, x, y);
                    }
                }
                if ((difficulty == 3 && timer >= 3500) || (difficulty == 2 && timer >= 2100))
                {
                    break;
                }
                timer++;
                Console.WriteLine(timer);
                ClearWindow();
                FillGame(Cards, x, y);
                DisplayWindow();
                Delay(1);
            }
            if (difficulty == 3 && timer >= 3500 || (difficulty == 2 && timer >= 2100))
            {
                GameOver();
            }
        }

        static void ClickGame(Card[,] Cards, int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (MouseX >= Cards[i, j].x && MouseX <= Cards[i, j].x + Cards[i, j].sizeX && MouseY >= Cards[i, j].y 
                        && MouseY <= Cards[i, j].y + Cards[i, j].sizeY && Cards[i, j].status == 0)
                    {
                        PlaySound(cardSwapS, 80);
                        Cards[i, j].status = 1;
                    }
                }
            }
        }

        static int StatusCheck(Card[,] Cards, int x, int y)
        {
            int result = 0;
            int status0Count = 0;
            int status1Count = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (Cards[i, j].status == 1) status1Count++;
                    if (Cards[i, j].status == 0) status0Count++;
                }
            }
            if (status1Count == 2)
            {
                result = 1;
            }
            else if (status0Count == 0 && status1Count == 0)
            {
                result = 2;
            }
            return result;
        }

        static void StatusChange(Card[,] Cards, int x, int y)
        {
            int[] coord = new int[2];
            int status1Count = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (Cards[i, j].status == 1)
                    {
                        status1Count++;
                        if (status1Count == 1)
                        {
                            coord[0] = i;
                            coord[1] = j;
                        }
                        else
                        {
                            if (Cards[i, j].value == Cards[coord[0], coord[1]].value)
                            {
                                Cards[i, j].status = 2;
                                Cards[coord[0], coord[1]].status = 2;
                            }
                            else
                            {
                                Cards[i, j].status = 0;
                                Cards[coord[0], coord[1]].status = 0;
                            }
                        }
                    }
                }
            }
        }

        static void FillGame(Card[,] Cards, int x, int y)
        {
            SetFillColor(50, 0, 255);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (Cards[i, j].status < 2)
                    {
                        FillRectangle(Cards[i, j].x, Cards[i, j].y, Cards[i, j].sizeX, Cards[i, j].sizeY);
                    }
                    if (Cards[i, j].status == 1)
                    {
                        DrawSprite(icons[Cards[i, j].value], Cards[i, j].x, Cards[i, j].y);
                    }
                }
            }
        }

        static void GameOver()
        {
            int timer = 0;
            while (timer < 200)
            {
                DispatchEvents();
                ClearWindow();
                SetFillColor(255, 255, 255);
                DrawText(200, 250, "Время Вышло!", 60);
                DisplayWindow();
                Delay(1);
                timer++;
            }
        }
    }
}