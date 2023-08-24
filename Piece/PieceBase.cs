using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PieceBase : ScriptableObject
{
    //基礎データ
    [SerializeField] int id;
    [SerializeField] new string name;
    [SerializeField] PieceType type;
    [SerializeField] int startX;
    [SerializeField] int startY;
    [SerializeField] int top;
    [SerializeField] int topRight;
    [SerializeField] int right;
    [SerializeField] int bottomRight;
    [SerializeField] int bottom;
    [SerializeField] int bottomLeft;
    [SerializeField] int left;
    [SerializeField] int topLeft;
    [SerializeField] Sprite pieceIcon;
    [SerializeField] Sprite topIcon;
    [SerializeField] Sprite topRightIcon;
    [SerializeField] Sprite rightIcon;
    [SerializeField] Sprite bottomRightIcon;
    [SerializeField] Sprite bottomIcon;
    [SerializeField] Sprite bottomLeftIcon;
    [SerializeField] Sprite leftIcon;
    [SerializeField] Sprite topLeftIcon;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public PieceType Type { get => type; set => type = value; }
    public int StartX { get => startX; set => startX = value; }
    public int StartY { get => startY; set => startY = value; }
    public int Top { get => top; set => top = value; }
    public int TopRight { get => topRight; set => topRight = value; }
    public int Right { get => right; set => right = value; }
    public int BottomRight { get => bottomRight; set => bottomRight = value; }
    public int Bottom { get => bottom; set => bottom = value; }
    public int BottomLeft { get => bottomLeft; set => bottomLeft = value; }
    public int Left { get => left; set => left = value; }
    public int TopLeft { get => topLeft; set => topLeft = value; }
    public Sprite PieceIcon { get => pieceIcon; set => pieceIcon = value; }
    public Sprite TopIcon { get => topIcon; set => topIcon = value; }
    public Sprite TopRightIcon { get => topRightIcon; set => topRightIcon = value; }
    public Sprite RightIcon { get => rightIcon; set => rightIcon = value; }
    public Sprite BottomRightIcon { get => bottomRightIcon; set => bottomRightIcon = value; }
    public Sprite BottomIcon { get => bottomIcon; set => bottomIcon = value; }
    public Sprite BottomLeftIcon { get => bottomLeftIcon; set => bottomLeftIcon = value; }
    public Sprite LeftIcon { get => leftIcon; set => leftIcon = value; }
    public Sprite TopLeftIcon { get => topLeftIcon; set => topLeftIcon = value; }

    public enum PieceType
    {
        KingSlime,
        Slime,
        GoldSlime,
        SlimeTower,
        AngelSlime,
        MetalSlime,
        HealSlime,
        EarthSlime,
        StrayMetal,
        BubbleSlime,
        GoldTotem,
        MetalKing,
        HealKing,
        SunSlime,
        StrayMealKing
    }
}
