using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Script, returning the input, used to the CharController Script, which converts the given information into actions the player can do
/// </summary>
public class PlayerInput : MonoBehaviour, IInput
{
    // The input for horizontal movement
    public float Horizontal
    {
        get
        {
            return Input.GetAxis("Horizontal");
        }
    }

    #region Actions

    // Check if jump button is pressed or holded
    public int Jump
    {
        get
        {
            if (Input.GetButtonDown("Jump"))
            {
                return 1;
            }
            else if (Input.GetButton("Jump"))
            {
                return 2;
            }
            return 0;
        }
    }

    // Looks for Input for Dodge
    public bool Dodge
    {
        get
        {
            if (Input.GetButtonDown("Dodge"))
            {
                return true;
            }
            return false;
        }
    }

    // Looks for Input for Attack
    public bool Shoot
    {
        get
        {
            if (Input.GetButton("Shoot"))
            {
                return true;
            }
            return false;
        }
    }

    // Looks for Input for Picking up
    public bool Interact
    {
        get
        {
            if (Input.GetButtonDown("Interact"))
            {
                return true;
            }
            return false;
        }
    }

    #endregion
}
