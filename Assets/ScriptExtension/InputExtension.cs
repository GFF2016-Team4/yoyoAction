using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputExtension
{
    public static void GetAxisVelocity(out Vector2 input)
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
    }

    public static void GetAxisVelocityRaw(out Vector2 input)
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }
}
