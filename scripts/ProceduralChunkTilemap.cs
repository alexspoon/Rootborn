using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class ProceduralChunkTilemap : TileMapLayer
{
    [ExportToolButton("GenerateImageMap")]
    public Callable ImageButton => Callable.From(GenerateImageMap);
    [ExportToolButton("ClearImageMap")]
    public Callable ClearButton => Callable.From(ClearImage);
    [ExportToolButton("ConnectTiles")]
    public Callable ConnectButton => Callable.From(ConnectTiles);
    [ExportToolButton("RerollSeed")]
    public Callable RerollButton => Callable.From(UpdateSeed);
    [ExportToolButton("RerollAndGenerate")]
    public Callable RerollAndGenerateButton => Callable.From(RerollAndGenerate);
    [Signal] private delegate void ChunkGeneratedEventHandler(Vector2I coord);

    [ExportGroup("General Properties")]
    [Export] public bool PrintDebugInfo = true;
    [Export] public uint Seed = 0;
    [Export] public int ChunkSize = 16;
    [Export] public int TileSize = 16;
    [Export] public int RenderDistance = 1;

    [ExportSubgroup("Noise Properties")]
    [Export] private NoiseTexture2D WorldGenNoiseTexture;
    [Export] private NoiseTexture2D OreGenNoiseTexture;
    [Export] private Image worldSprite;

    private RandomNumberGenerator _rng = new();
    private Player _player;
    private Queue<Vector2I> _placeQueue = [];
    private Queue<Vector2I> _eraseQueue = [];
    private Queue<Vector2I> _smoothQueue = [];
    private Queue<Vector2I> _connectQueue = [];

    public override void _Ready()
    {
        UpdateSeed();
        //_player = GetParent().GetNodeOrNull<Player>("Player");
    }

    private void RerollAndGenerate()
    {
        UpdateSeed();
        GenerateImageMap();
    }

    private void GenerateImageMap()
    {
        if (_placeQueue.Count != 0) return;
        GD.Print("image world");
        ClearImage();
        lock (worldSprite)
        {
            var width = worldSprite.GetWidth();
            var height = worldSprite.GetHeight();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixelColor = worldSprite.GetPixel(x, y);
                    var coord = new Vector2I(x, y);
                    switch (ColorToTile(pixelColor))
                    {
                        case "Filled":
                            _placeQueue.Enqueue(coord);
                            break;
                        case "Empty":
                            break;
                        case "ProceduralDirt":
                            //make place only dirt
                            var noiseValueDirt = WorldGenNoiseTexture.Noise.GetNoise2D(x, y);
                            if (noiseValueDirt < -0.32) _placeQueue.Enqueue(coord);
                            break;
                        case "ProceduralStone":
                            //make place only stone
                            var noiseValueStone = WorldGenNoiseTexture.Noise.GetNoise2D(x, y);
                            if (noiseValueStone < -0.32) _placeQueue.Enqueue(coord);
                            break;
                        case null:
                            break;
                    }
                }
            }
        }

    }

    private string ColorToTile(Color color)
    {
        if (color == Colors.Black) return "Filled";
        if (color == Colors.White) return "Empty";
        if (color == Color.Color8(88,48,18)) return "ProceduralDirt";
        if (color == Color.Color8(60,60,60)) return "ProceduralStone";
        if (color == Color.Color8(0, 120, 0)) return "ProceduralVegetable";
        return null;
    }

    private void ClearImage()
    {
        _placeQueue.Clear();
        var width = worldSprite.GetWidth();
        var height = worldSprite.GetHeight();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var coord = new Vector2I(x, y);
                EraseCell(coord);
            }
        }
    }

    private async void ConnectTiles()
    {
        var tiles = GetUsedCells();
        await Task.Run(() =>
        {
            SetCellsTerrainConnect(tiles, 0, 0);
        }); 
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            // GD.Print("running");
            PlaceTiles();
        }
    }

    private void PlaceTiles()
    {
        for (int i = 0; i < 16; i++)
        {
            if (_placeQueue.Count == 0)
            {
                return;
            }

            var coord = _placeQueue.Dequeue();
            SetCell(coord, 0, new Vector2I(2,4), 0);
        }
    }

    /// Utility methods

    /// <summary>
    /// If a seed has not been provided, randomise it and otherwise set the WorldGenNoiseTexture and OreGenNoiseTexture Noise seed to the provided seed.
    /// </summary>
    private void UpdateSeed()
    {
        if (Seed == 0)
        {
            WorldGenNoiseTexture.Noise.Set("seed", _rng.Randi());
            OreGenNoiseTexture.Noise.Set("seed", _rng.Randi());
        }
        else
        {
            WorldGenNoiseTexture.Noise.Set("seed", Seed);
            OreGenNoiseTexture.Noise.Set("seed", Seed);
        }
    }
    /// <summary>
    /// Takes a coordinate and if the the tile at that coordinate is empty, returns true; otherwise it returns false.
    /// </summary>
    private bool IsTileEmpty(Vector2I coord)
    {
        if (GetCellAlternativeTile(coord) == -1) return true;
        else return false;
    }
    /// <summary>
    /// Checks the 8 tiles surrounding the given coordinate, and returns the number of tiles that are not empty.
    /// </summary>
    private int GetSurroundingTileCount(Vector2I coords)
    {
        int count = 0;
        for (int neighbourX = coords.X - 1; neighbourX <= coords.X + 1; neighbourX++)
        {
            for (int neighbourY = coords.Y - 1; neighbourY <= coords.Y + 1; neighbourY++)
            {
                {
                    if (neighbourX != coords.X || neighbourY != coords.Y)
                    {
                        var tileFilled = IsTileEmpty(new Vector2I(neighbourX, neighbourY));
                        if (!tileFilled) count++;
                    }
                }
            }
        }
        return count;
    }

    /// Chunk generation methods

    /// <summary>
    /// Takes a coordinate and generates a (ChunkSize x ChunkSize) chunk from that coordinate.
    /// </summary>
    public async Task LoadChunk(Vector2I coord)
    {
        await Task.Run(() =>
        {
            for (int x = coord.X; x < ChunkSize; x++)
            {
                for (int y = coord.Y; y < ChunkSize; y++)
                {
                    var tileCoord = new Vector2I(x, y);
                    if (WorldGenNoiseTexture.Noise.GetNoise2D(x, y) < -0.32)
                    {
                        _placeQueue.Enqueue(tileCoord);
                    }
                }
            }
        });
    }
    /// <summary>
    /// Takes a coordinate and erases a (ChunkSize x ChunkSize) chunk from that coordinate.
    /// </summary>
    public async Task UnloadChunk(Vector2I coord)
    {
        await Task.Run(() =>
        {
            for (int x = coord.X; x < ChunkSize; x++)
            {
                for (int y = coord.X; y < ChunkSize; y++)
                {
                    var tileCoord = new Vector2I(x, y);
                    _eraseQueue.Enqueue(tileCoord);
                }
            }
        });
    }

}
