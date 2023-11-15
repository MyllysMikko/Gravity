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

        List<Texture> tileSets = new List<Texture>();

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

            map = MapReader.LoadMapFromFile("Map/testmap2.tmj");
            foreach (var tilesetFile in map.tilesetFiles)
            {
                string fileName = tilesetFile.imageWoPath;
                Texture texture = Raylib.LoadTexture($"Map/Tilesets/{fileName}");
                tileSets.Add(texture);
            }
            if (map != null)
            {
                Console.WriteLine("Map loaded!");
                map.PrintToConsole();
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
            DrawMap();
            player.Draw();
            Raylib.EndDrawing();
        }

        void DrawMap()
        {
            for (int i = 0; i < map.layers.Count; i++)
            //oreach(var layer in map.layers)
            {
                for (int row = 0; row < map.height; row++)
                {
                    for (int col = 0; col < map.width; col++)
                    {
                        int tileId = map.layers[i].data[row * map.layers[i].width + col];
                        int x = col * map.tilewidth;
                        int y = row * map.tileheight;
                        DrawTile(i, x, y, tileId);
                    }
                }
            }
        }

        void DrawTile(int index, int x, int y, int TileId)
        {
            //Tiled aloittaa laskennan yhdestä.
            TileId -= 1;
            int tilesPerRow = map.tilesetFiles[index].imagewidth / map.tilewidth;
            //Console.WriteLine((float)TileId / (float)tilesPerRow);
            float rowf = MathF.Floor(TileId / tilesPerRow);

            int row = Convert.ToInt32(rowf);
            int column = TileId % tilesPerRow;

            int u = column * map.tilewidth;
            int v = row * map.tileheight;

            Raylib.DrawTextureRec(tileSets[index], new Rectangle(u, v, map.tilewidth, map.tileheight), new Vector2(x, y), Raylib.WHITE);

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
