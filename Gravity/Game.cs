using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using TurboMapReader;

namespace Gravity
{
    internal class Game
    {
        int window_width = 960;
        int window_height = 720;
        Player player;
        
        TiledMap map;

        Stack<GameState> gameStates = new Stack<GameState>();


        public void Start()
        {
            Init();
            LoadLevel();
            Loop();
        }

        void Init()
        {
            Raylib.SetTargetFPS(60);
            Raylib.InitWindow(window_width, window_height, "Gravity");

            player = new Player(new Vector2(0, 0), 100, 50);

            gameStates.Push(GameState.Alive);

        }

        void LoadLevel()
        {
            //Lataa kenttä
            Vector2 playerPos = new Vector2(window_width * 0.5f, window_height * 0.5f);
            player.transform.position = playerPos;

            map = MapReader.LoadMapFromFile("Map/testmap.tmj");
            if (map != null)
            {
                Console.WriteLine("Map loaded!");
            }
        }

        void ResetLevel()
        {
            //Aseta pelaaja ja muut "objektit" paikalleen
        }



        void Loop()
        {
            while(!Raylib.WindowShouldClose())
            {
                switch (gameStates.Peek())
                {
                    case GameState.MainMenu:
                        break;

                    case GameState.Pause:
                        break;

                    case GameState.Options:
                        break;

                    case GameState.Alive:
                        Update();
                        Draw();
                        break;

                    default:
                        break;
                }
            }
        }

        void Update()
        {
            player.Update();
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLANK);
            player.Draw();
            Raylib.EndDrawing();
        }

        void DrawMap()
        {
            foreach(var layer in map.layers)
            {
                for (int row = 0; row < map.height; row++)
                {
                    for (int col = 0; col < map.width; col++)
                    {
                        int tileId = layer.data[row * layer.width + col];
                        int x = col * map.tilewidth;
                        int y = row * map.tileheight;
                        //DrawTile(x, y, tileId);
                    }
                }
            }
        }




        enum GameState
        {
            MainMenu,
            Pause,
            Options,
            Alive
        }
    }
}
