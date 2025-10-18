using System.Numerics;
using SDL;

namespace PinkFox.Graphics.Rendering;

/// <summary>
/// Represents a texture atlas that divides a texture into a grid of equally sized cells, 
/// supporting optional padding and spacing between cells.
/// </summary>
public class TextureAtlas
{
    /// <summary>
    /// The underlying texture used by this atlas.
    /// </summary>
    public Texture2D TextureHandle { get; }

    /// <summary>
    /// Read-only collection of all source rectangles corresponding to atlas cells.
    /// </summary>
    public ReadOnlySpan<SDL_FRect> Cells => _Cells;
    public Vector2 CellSize => new(_CellWidth, _CellHeight);

    private readonly SDL_FRect[] _Cells;
    private readonly float _TextureWidth;
    private readonly float _TextureHeight;

    private readonly float _CellWidth;
    private readonly float _CellHeight;
    private readonly int _CellsPerColumn;
    private readonly int _CellsPerRow;
    private readonly int _NumberOfCells;

    private readonly float _Padding;
    private readonly float _Spacing;

    /// <summary>
    /// Constructs a new texture atlas.
    /// </summary>
    /// <param name="texture">The texture to be divided into atlas cells.</param>
    /// <param name="cellsPerRow">Number of cells horizontally in the atlas.</param>
    /// <param name="cellsPerColumn">Number of cells vertically in the atlas.</param>
    /// <param name="padding">Inner padding (in pixels) applied inside each cell.</param>
    /// <param name="spacing">Spacing (in pixels) between cells.</param>
    /// <exception cref="ArgumentException">Thrown if the padding is too large for the calculated cell size.</exception>
    public TextureAtlas(Texture2D texture, int cellsPerRow, int cellsPerColumn, float padding = 0, float spacing = 0)
    {
        TextureHandle = texture;
        _TextureWidth = texture.Width;
        _TextureHeight = texture.Height;

        _CellsPerRow = cellsPerRow;
        _CellsPerColumn = cellsPerColumn;
        _Padding = padding;
        _Spacing = spacing;

        _CellWidth = (_TextureWidth - spacing * (_CellsPerRow - 1)) / _CellsPerRow;
        _CellHeight = (_TextureHeight - spacing * (_CellsPerColumn - 1)) / _CellsPerColumn;

        if (_Padding * 2 > _CellWidth || _Padding * 2 > _CellHeight)
        {
            throw new ArgumentException("Padding is too large for the cell size.");
        }

        _NumberOfCells = _CellsPerRow * _CellsPerColumn;

        _Cells = GetAllCells();
    }

    /// <summary>
    /// Pre-computes and returns an array of source rectangles for all cells in the atlas.
    /// </summary>
    /// <returns>An array of <see cref="SDL_FRect"/> representing each cell's source rectangle.</returns>
    private SDL_FRect[] GetAllCells()
    {
        SDL_FRect[] cells = new SDL_FRect[_NumberOfCells];
        for (int row = 0; row < _CellsPerColumn; row++)
        {
            for (int column = 0; column < _CellsPerRow; column++)
            {
                int index = row * _CellsPerRow + column;

                float x = column * (_CellWidth + _Spacing) + _Padding;
                float y = row * (_CellHeight + _Spacing) + _Padding;
                float w = _CellWidth - (_Padding * 2);
                float h = _CellHeight - (_Padding * 2);

                cells[index] = new()
                {
                    x = x,
                    y = y,
                    w = w,
                    h = h
                };
            }
        }

        return cells;
    }


    /// <summary>
    /// Gets the source rectangle for a given cell index.
    /// </summary>
    /// <param name="index">The linear index of the cell (row-major order).</param>
    /// <returns>The source rectangle corresponding to the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of valid range.</exception>
    public SDL_FRect GetSourceRect(int index)
    {
        if (index < 0 || index >= _NumberOfCells)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Atlas index out of range.");
        }

        return _Cells[index];
    }

    /// <summary>
    /// Gets the source rectangle for a given cell by row and column coordinates.
    /// </summary>
    /// <param name="row">The zero-based row index of the cell.</param>
    /// <param name="column">The zero-based column index of the cell.</param>
    /// <returns>The source rectangle corresponding to the specified row and column.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the row or column are out of valid range.</exception>
    public SDL_FRect GetSourceRect(int row, int column)
    {
        int index = row * _CellsPerRow + column;
        if (index < 0 || index >= _NumberOfCells)
        {
            throw new ArgumentOutOfRangeException("Atlas coordinates out of range.");
        }

        return _Cells[index];
    }
}