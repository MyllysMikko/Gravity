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

        List<Rectangle> hitTiles = new List<Rectangle>();


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

            map = MapReader.LoadMapFromFile("Map/room.tmj");
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
                        Draw();
                        Update();
                        break;

                    default:
                        break;
                }
            }
        }

        void Update()
        {
            Vector2 previousPosition = player.transform.position;
            player.Update();
            Vector2 movement = new Vector2(player.transform.position.X - previousPosition.X, player.transform.position.Y - previousPosition.Y);

            Vector2 playerTilePosition = new Vector2(player.transform.position.X / map.tilewidth, player.transform.position.Y / map.tileheight);
            Vector2 playerMaxTilePosition = new Vector2(
                (player.transform.position.X + player.collision.size.X) / map.tilewidth,
                (player.transform.position.Y + player.collision.size.Y) / map.tileheight);

            List<Vector2> hitTiles = new List<Vector2>();

            for (int px = (int)playerTilePosition.X; px < playerMaxTilePosition.X + 1; px++)
            {
                for (int py = (int)playerTilePosition.Y; py < playerMaxTilePosition.Y; py++)
                {
                    int tileId = map.layers[0].data[py * map.layers[0].width + px];

                    if (tileId != 0)
                    {
                        hitTiles.Add(new Vector2(px, py));
                        //Console.WriteLine("Collision!");
                    }
                }
            }

            if (hitTiles.Count > 0)
            {
                CheckCollisions(hitTiles, movement);
            }

            //Console.WriteLine($"{playerTilePosition.X}, {playerTilePosition.Y} : {playerMaxTilePosition.X}, {playerMaxTilePosition.Y}");



        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLANK);
            DrawMap();
            Raylib.DrawText($"{Raylib.GetFPS()}", 0, 0, 50, Raylib.LIME);

            player.Draw();
            Raylib.EndDrawing();
        }

        void DrawMap()
        {
            hitTiles.Clear();
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
            if (TileId != 0)
            {
                TileId -= 1;
                int tilesPerRow = map.tilesetFiles[index].imagewidth / map.tilewidth;
                //Console.WriteLine((float)TileId / (float)tilesPerRow);
                float rowf = MathF.Floor(TileId / tilesPerRow);

                int row = Convert.ToInt32(rowf);
                int column = TileId % tilesPerRow;

                int u = column * map.tilewidth;
                int v = row * map.tileheight;


                Raylib.DrawTextureRec(tileSets[index], new Rectangle(u, v, map.tilewidth, map.tileheight), new Vector2(x, y), Raylib.WHITE);

                //Rectangle tileRec = new Rectangle(x, y, map.tilewidth, map.tileheight);
                //
                //bool collision = Raylib.CheckCollisionRecs(tileRec, player.GetRec());
                //
                //if (collision)
                //{
                //    hitTiles.Add(tileRec);
                //}
            }
        }

        void CheckCollisions(List<Vector2> hitTiles, Vector2 movement)
        {
            Rectangle playerRec = player.GetRec();
            foreach (var tile in hitTiles)
            {
                Rectangle tileRectangle = new Rectangle(tile.X * map.tilewidth, tile.Y * map.tileheight, map.tilewidth, map.tileheight);
                if (Raylib.CheckCollisionRecs(playerRec, tileRectangle))
                {
                    Rectangle collision = Raylib.GetCollisionRec(playerRec, tileRectangle);
                    //Console.WriteLine($"{collision.width}, {collision.height}");

                    Vector2 playerPos = player.transform.position;
                    Vector2 positionCorrection = new Vector2();

                    if (movement.X > 0)
                    {
                        positionCorrection.X = -collision.width;
                    }
                    else if (movement.X < 0)
                    {
                        positionCorrection.X = collision.width;
                    }

                    //if (movement.Y > 0)
                    //{
                    //    positionCorrection.Y = -collision.height;
                    //}

                    Console.WriteLine($"{positionCorrection.X}, {positionCorrection.Y}");

                    player.transform.position = playerPos + positionCorrection;
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
