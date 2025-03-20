namespace _250319_LeeHyungwon_Project_X
{
    internal class Program
    {
        // 게임의 기획의도 : 미로 게임이지만, 전체적인 맵이 보이지는 않는 형태 - 플레이어 기준 8방향의 맵만 보이게 설계
        struct Position
        {
            public int x;
            public int y;
        }
        static void Main(string[] args)
        {
            Opening();  // 시작 화면
            bool reset = true;  // 리셋을 위한 bool값

            while (reset == true)
            {
                bool gameOver = false;  // 게임오버 여부
                Position playerPos;     // 캐릭터 좌표
                playerPos.x = 0;
                playerPos.y = 0;
                Position goalPos;       // 골인 지점 좌표
                int score = 0;          // 먹은 코인 갯수(점수)
                bool getCoin = false;   // 코인을 획득했을 때(true)

                Start(ref playerPos, out goalPos, out bool[,] map, out bool[,] coin);

                while (gameOver == false)   // 게임 종료가 아닐 시
                {
                    Render(playerPos, goalPos, map, coin, ref getCoin);
                    ConsoleKey key = Input();
                    Update(key, ref playerPos, goalPos, map, coin, ref gameOver, ref score, ref getCoin);
                }
                End(score, ref reset);
            }
        }

        static void Opening()   // 시작 화면 출력
        {
            Console.WriteLine("========================");
            Console.WriteLine(" 고난이도 미로찾기 게임 ");
            Console.WriteLine("========================");
            Console.WriteLine();
            Console.WriteLine("아무 키나 눌러서 진행해주세요...");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("숨겨진 코인 개수는 몇개일까요?"); // 코인을 히든 요소로 설정
            Console.ResetColor();

            Console.ReadKey(true);
            Console.Clear();
        }


        static void Start(ref Position playerPos, out Position goalPos, out bool[,] map, out bool[,] coin)
        {
            Console.CursorVisible = false;  // 커서 안보이게 초기 설정
            playerPos.x = 1;
            playerPos.y = 1;

            goalPos.x = 13;
            goalPos.y = 8;

            map = new bool[10, 15]      // 전체 맵 (벽 표시)
            {
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, true, false, true, true, true, true, true, true, true, true, true, true, true, false },
                {false, true, false, true, false, false, false, true, false, false, false, true, false, true, false },
                {false, true, false, true, true, true, false, true, false, true, false, false, false, true, false },
                {false, true, true, true, false, false, false, false, false, true, true, true, true, true, false },
                {false, false, false, true, false, true, true, true, true, true, false, false, false, false, false },
                {false, true, true, true, false, false, false, true, false, false, false, true, true, true, false },
                {false, true, false, false, false, true, true, true, true, true, false, true, false, true, false },
                {false, true, true, true, false, true, false, true, false, true, true, true, false, true, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
            };

            coin = new bool[10, 15]     // 전체 코인 위치
            {
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, true, false, false, false },
                {false, false, false, false, false, true, false, true, false, true, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, true, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                {false, false, false, true, false, true, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            };
        }

        static void Render(Position playerPos, Position goalPos, bool[,] map, bool[,] coin, ref bool getCoin)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            PrintMap(playerPos, map);   // 맵 출력            
            PrintPlayer(playerPos);     // 플레이어 출력            
            PrintGoalPos(playerPos, goalPos);   // 골인지점 출력            
            GetCoinWrite(ref getCoin);          // 코인 획득 시 출력            
            PrintCoin(playerPos, coin);         // 코인 위치 출력

        }
        #region Render 내 함수

        static void PrintMap(Position playerPos, bool[,] map)   // 맵 출력
        {
            // 플레이어의 위치 기준으로 상하좌우 대각선 4방향(8방향)의 거리만 보이도록 설계
            for (int y = playerPos.y - 1; y <= playerPos.y + 1; y++)
            {
                for (int x = playerPos.x - 1; x <= playerPos.x + 1; x++)
                {
                    if (map[y, x] == false)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
            }
        }

        static void PrintPlayer(Position playerPos)     // 플레이어 출력
        {
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('P');
            Console.ResetColor();
        }

        static void PrintGoalPos(Position playerPos, Position goalPos)  // 골인 지점 출력
        {
            // 플레이어 시야-2칸 거리에 들어왔을 때 보이도록 설정
            for (int y = playerPos.y - 2; y <= playerPos.y + 2; y++)
            {
                for (int x = playerPos.x - 2; x <= playerPos.x + 2; x++)
                {
                    if (x == goalPos.x && y == goalPos.y)
                    {
                        Console.SetCursorPosition(goalPos.x, goalPos.y);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('G');
                        Console.ResetColor();
                    }
                }
            }
        }

        static void PrintCoin(Position playerPos, bool[,] coin) // 코인 출력
        {
            // 플레이어의 위치 기준으로 상하좌우 대각선 4방향(8방향)의 거리만 보이도록 설계
            for (int y = playerPos.y - 1; y <= playerPos.y + 1; y++)
            {
                for (int x = playerPos.x - 1; x <= playerPos.x + 1; x++)
                {
                    if (coin[y, x] == true)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("*");
                        Console.ResetColor();
                    }
                }
            }
        }

        static void GetCoinWrite(ref bool getCoin)  // 코인 획득시 출력
        {
            if (getCoin == true)
            {
                Console.SetCursorPosition(1, 13);
                Console.WriteLine("코인을 획득했습니다.");
                getCoin = false;    // 코인 획득한 순간 한 번만 출력하고 다시 false로 전환
            }
        }

        #endregion

        static ConsoleKey Input()   // 입력
        {
            return Console.ReadKey(true).Key;
        }

        static void Update(ConsoleKey key, ref Position playerPos, Position goalPos, bool[,] map, bool[,] coin, ref bool gameOver, ref int score, ref bool getCoin)
        {
            Move(key, ref playerPos, map);      // 플레이어 움직임
            score = GetCoin(playerPos, coin, ref score, ref getCoin);     // 코인 획득 시에 따른 점수 증가 및 획득상태 처리
            bool isClear = isGameClear(playerPos, goalPos); // 게임 클리어 여부 처리
            if (isClear)    // 게임이 클리어 되었을 때
            {
                gameOver = true;
            }
        }
        #region Update 내 함수
        static void Move(ConsoleKey key, ref Position playerPos, bool[,] map)   // 플레이어 움직임
        {
            switch (key)
            {
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (map[playerPos.y, playerPos.x - 1] == true)
                    {
                        playerPos.x--;
                    }
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (map[playerPos.y, playerPos.x + 1] == true)
                    {
                        playerPos.x++;
                    }
                    break;

                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (map[playerPos.y - 1, playerPos.x] == true)
                    {
                        playerPos.y--;
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (map[playerPos.y + 1, playerPos.x] == true)
                    {
                        playerPos.y++;
                    }
                    break;
            }
        }

        static int GetCoin(Position playerPos, bool[,] coin, ref int score, ref bool getCoin)   // 코인 획득 판정
        {
            if (coin[playerPos.y, playerPos.x] == true) // 플레이어가 코인과 좌표가 겹쳤을 때(코인에 도달했을 때)
            {
                score++;    // 획득갯수(score 상승)
                getCoin = true;     // 코인을 획득상태 = true
                coin[playerPos.y, playerPos.x] = false; // 해당 위치의 얻은 코인은 사라짐                
            }
            return score;
        }

        static bool isGameClear(Position playerPos, Position goalPos)   // 게임 승리 여부 판정
        {
            bool success = (playerPos.x == goalPos.x) && (playerPos.y == goalPos.y);    // 플레이어가 골인지점 도달시
            return success;
        }

        #endregion

        static void End(int score, ref bool reset)  // 게임 종료
        {
            Console.Clear();
            Console.WriteLine("축하합니다!!! 게임을 클리어하였습니다!");
            Console.WriteLine("모은 코인의 개수는 {0}개입니다.", score);
            if (score == 7) // 코인을 전부 획득했을 때
            {
                Console.WriteLine();
                Console.WriteLine("축하합니다!!! 숨겨진 코인을 전부 찾으셨습니다!");
            }
            else // 코인을 전부 획득하지 못했을 때
            {
                Console.WriteLine();
                Console.WriteLine("찾지 못한 코인 : {0}", 7 - score);
                Console.WriteLine();
                Console.WriteLine("다시 찾아보실까요?");
            }

            Console.WriteLine();
            Console.WriteLine("다시 시작하려면 R, 종료하려면 T를 눌러주세요.");

            while (reset == true) // 리셋 기능 구현
            {
                if (Console.ReadKey(true).Key == ConsoleKey.T)  // T를 누르면 종료
                {
                    reset = false;
                }
                else if (Console.ReadKey(true).Key == ConsoleKey.R) // R을 누르면 End구문을 빠져나가 게임 재시작 가능
                {
                    return;
                }
                // else - 나머지 키를 입력할 경우 이 구문에서 못 빠져나가게 하여, 입력 시 무반응으로 처리
            }
        }
    }
}
