using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class NetworkTransform2D : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}