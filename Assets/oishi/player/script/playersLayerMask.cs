using UnityEngine;

public class playersLayerMask
{
    public const string player = "Player";
    public const string normalRope = "Rope/Normal";
    public const string lockRope = "Rope/Lock";
    public const string catchRope = "Rope/Catch";
    public const string bullet = "Bullet";

    public static int PlayerAndRopes
    {
        get
        {
            return LayerMask.GetMask(player, bullet, normalRope, lockRope, catchRope);
        }
    }

    public static int Player
    {
        get
        {
            return LayerMask.NameToLayer(player);
        }
    }

    public static int Ropes
    {
        get
        {
            return LayerMask.GetMask(normalRope, lockRope, catchRope, bullet);
        }
    }

    public static int IgnorePlayerAndRopes
    {
        get { return -1 - (PlayerAndRopes); }
    }

    public static int IgnorePlayer
    {
        get { return -1 - (Player); }
    }

    public static int IgnoreRopes
    {
        get { return -1 - (Ropes); }
    }
}