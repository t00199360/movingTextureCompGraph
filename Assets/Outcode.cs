using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcode
{

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public Outcode(Vector2 vPoint)
    {
        up = vPoint.y > 1;                  //constructor 3
        down = vPoint.y < -1;
        left = vPoint.x < -1;
        right = vPoint.x > 1;
    }
    public Outcode()
    {
        up = false;                         //constructor 1
        down = false;
        left = false;
        right = false;
    }
    public Outcode(bool Up, bool Down, bool Left, bool Right)
    {
        up = Up;                            //constructor 2
        down = Down;
        left = Left;
        right = Right;
    }

    /// <summary>
    /// Logical EQUALS
    /// </summary>
    public static bool operator ==(Outcode a, Outcode b)            //correct
    {
        return (a.up == b.up) && (a.down == b.down) && (a.left == b.left) && (a.right == b.right);
    }
    /// <summary>
    /// Logical AND
    /// </summary>
    public static Outcode operator *(Outcode a, Outcode b)             //and correct
    {
        return new Outcode(a.up && b.up, a.down && b.down, a.left && b.left, a.right && b.right);
    }
    /// <summary>
    /// Logical OR
    /// </summary>
    public static Outcode operator +(Outcode a, Outcode b)            
    {
        return new Outcode(a.up || b.up, a.down || b.down, a.left || b.left, a.right || b.right);
    }
    /// <summary>
    /// NOT EQUALS
    /// </summary>
    public static bool operator != (Outcode a, Outcode b)                 //correct
    {
        return !(a == b);
    }

   

}
