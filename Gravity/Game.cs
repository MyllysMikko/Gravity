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

        Texture tileSet;

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

            player = new Player(new Vector2(0, 0), 300, 50);

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
                tileSet = Raylib.LoadTexture($"Map/Tilesets/{fileName}");
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


        /// <summary>
        /// Switch ja stack jäivät käyttämättömäksi sillä en ehtinyt tehdä menuja.
        /// </summary>
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
            Vector2 previousPosition = player.transform.position;
            player.Update();
            Vector2 movement = new Vector2(player.transform.position.X - previousPosition.X, player.transform.position.Y - previousPosition.Y);

            
            List<Vector2> hitTiles = new List<Vector2>();

            if (GetNearbyTiles(out hitTiles))
            {
                CheckCollisions(hitTiles, movement);
            }
  

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

        /// <summary>
        /// Piirtää kartan
        /// </summary>

        void DrawMap()
        {
            hitTiles.Clear();
            for (int i = 0; i < map.layers.Count; i++)
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

        /// <summary>
        /// Piirtää halutun Tilen haluttuun kordinaattiin
        /// </summary>
        /// <param name="index">Kartan layer josta piirretään</param>
        /// <param name="x">Tilen X-kordinaatti</param>
        /// <param name="y">Tilen Y-kordinaatti</param>
        /// <param name="TileId">Piirrettävä tile</param>
        void DrawTile(int index, int x, int y, int TileId)
        {
            if (TileId != 0)
            {
                TileId -= 1;
                int tilesPerRow = map.tilesetFiles[index].imagewidth / map.tilewidth;
                float rowf = MathF.Floor(TileId / tilesPerRow);

                int row = Convert.ToInt32(rowf);
                int column = TileId % tilesPerRow;

                int u = column * map.tilewidth;
                int v = row * map.tileheight;


                Raylib.DrawTextureRec(tileSet, new Rectangle(u, v, map.tilewidth, map.tileheight), new Vector2(x, y), Raylib.WHITE);

            }
        }


        /// <summary>
        /// Ottaa pelaajan läheisyydeltä tilet joihin tämä voisi törmätä.
        /// Funktio palauttaa True mikäli mahdollisia törmäyksiä on havaittu. Muuten palautetaan False.
        /// 
        /// Funktio palauttaa listan mahdollisistä törmäyksistä out parametriin.
        /// 
        /// </summary>
        /// <param name="returnHitTiles">Muuttuja johon lista mahdollisista törmäyksistä sijoitetaan</param>
        /// <returns></returns>
  
        // Matikassa on jotain pielessä sillä se reagoi seiniin hieman ennenkuin pelaaja oikeasti koskettaa, mutta toimii oikein Y-Akselilla.
        // Tämä kuitenkin on varmaa fine, sillä me myöhemmin käydään nämä tilet läpi jolloin testataan ollaako oikeasti törmätty.
        bool GetNearbyTiles(out List<Vector2> returnHitTiles)
        {
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
                    }
                }
            }

            returnHitTiles = hitTiles;

            if (hitTiles.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Käy lapi annetut Tilet ja käy läpi onko pelaaja törmännyt näihin.
        /// Pelaajan liikettä käytetään tarkistamaan onko törmäys järkevä.
        /// </summary>
        /// <param name="hitTiles">Tarkistettavat Tilet</param>
        /// <param name="movement">Pelaajan liike</param>
        
        //Koodi ei vielä reagoi siihen jos pelaaja kävelee reunalta alas. Onneksi testikenttä onkin suljettu laatikko!
        void CheckCollisions(List<Vector2> hitTiles, Vector2 movement)
        {

            foreach (var tile in hitTiles)
            {

                Rectangle playerRec = player.GetRec();
                Rectangle tileRectangle = new Rectangle(tile.X * map.tilewidth, tile.Y * map.tileheight, map.tilewidth, map.tileheight);
                if (Raylib.CheckCollisionRecs(playerRec, tileRectangle))
                {
                    Rectangle collision = Raylib.GetCollisionRec(playerRec, tileRectangle);

                    Vector2 playerPos = player.transform.position;
                    Vector2 positionCorrection = new Vector2();

                    bool touchedWall = false;

                    if (movement.X > 0)
                    {
                        if (collision.width <= movement.X)
                        {
                            positionCorrection.X = -collision.width;
                            touchedWall = true;
                        }
                    }
                    else if (movement.X < 0)
                    {
                        if (collision.width <= -movement.X)
                        {
                            positionCorrection.X = collision.width;
                            touchedWall = true;
                        }

                    }

                    collision = Raylib.GetCollisionRec(player.GetRec(), tileRectangle);

                    //Kolme if-lausetta siäkkäin tuntuu vähä oudolle. Kuitenkin toimii!
                    if (!touchedWall)
                    {
                        if (movement.Y > 0)
                        {
                            if (collision.height <= movement.Y)
                            {
                                player.inAir = false;
                                positionCorrection.Y = -collision.height;
                            }

                        }
                        else if (movement.Y < 0)
                        {
                            if (collision.height <= -movement.Y)
                            {
                                player.inAir = false;
                                positionCorrection.Y = collision.height;
                            }
                        }
                    }

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
