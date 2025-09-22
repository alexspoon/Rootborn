using Godot;
using System;

public static class UtilityFunctions
{
    public static Timer CreateOneShotTimer(float waitTime, Node node)
    {
        var root = node.GetTree().Root;
        Timer timer = new();
        timer.WaitTime = waitTime;
        timer.OneShot = true;
        root.AddChild(timer);
        timer.Start();
        return timer;
    }
}
