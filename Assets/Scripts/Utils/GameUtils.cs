using System;
using UnityEngine;

public static class GameUtils
{
    public static string GetShortStringId(Guid guid, int lenght = 5)
    {
        var unicIdStr = guid.ToString();
        return unicIdStr[..Mathf.Min(unicIdStr.Length, lenght)];
    }
}
