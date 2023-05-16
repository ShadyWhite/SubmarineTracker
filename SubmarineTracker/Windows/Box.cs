﻿using System;
using System.Numerics;
using ImGuiNET;

namespace SubmarineTracker.Windows;

// From https://github.com/ocornut/imgui/issues/5944
public static class Box
{

    public struct Modifier
    {
        public Vector4 FPadding = new();
        public uint FBackgroundColor = 0;
        public uint FBorderColor = 0;

        public Modifier() { }

        // x/y/z/w
        public Modifier Padding(float iTop, float iRight, float iBottom, float iLeft)
        {
            FPadding =  new Vector4(iTop, iRight, iBottom, iLeft);
            return this;
        }

        public Modifier Padding(float iPadding)
        {
            return Padding(iPadding, iPadding, iPadding, iPadding);
        }

        public Modifier Padding(float iHorizontalPadding, float iVerticalPadding)
        {
            return Padding(iVerticalPadding, iHorizontalPadding, iVerticalPadding, iHorizontalPadding);
        }

        public Modifier BackgroundColor(uint iColor)
        {
            FBackgroundColor = iColor;
            return this;
        }
        public Modifier BorderColor(uint iColor)
        {
            FBorderColor = iColor;
            return this;
        }
    };

    private static bool ColorIsTransparent(uint iColor)
    {
        return iColor == 0;
    }

    public static void SimpleBox(Modifier iModifier, Action iBoxContent)
    {
        // Implementation note: this is made static because of this https://github.com/ocornut/imgui/issues/5944#issuecomment-1333930454
        // "using a new Splitter every frame is prohibitively costly".
        ImDrawListSplitter kSplitter = new();

        var hasBackground = !ColorIsTransparent(iModifier.FBackgroundColor);
        var hasBorder = !ColorIsTransparent(iModifier.FBorderColor);

        ImDrawListPtr drawList;
        if(hasBackground || hasBorder)
        {
            drawList = ImGui.GetWindowDrawList();

            // split draw list in 2
            drawList.ChannelsSplit(2);

            // first we draw in channel 1 to render iBoxContent (will be on top)
            drawList.ChannelsSetCurrent(1);
        }

        var min = ImGui.GetCursorScreenPos();
        // account for padding left/top
        ImGui.SetCursorScreenPos(min + new Vector2(iModifier.FPadding.W, iModifier.FPadding.X));

        ImGui.BeginGroup();
        {
            iBoxContent();
            ImGui.EndGroup();
        }

        // account for padding right/bottom
        var max = ImGui.GetItemRectMax() + new Vector2(iModifier.FPadding.Y, iModifier.FPadding.Z);

        if( drawList._Data != nint.Zero)
        {
            // second we draw the rectangle and border in channel 0 (will be below)
            drawList.ChannelsSetCurrent(0);

            // draw the background
            if(hasBackground)
                drawList.AddRectFilled(min, max, iModifier.FBackgroundColor);

            // draw the border
            if(hasBorder)
                drawList.AddRect(min, max, iModifier.FBorderColor);

            drawList.ChannelsMerge();
        }

        // reposition the cursor (top left) and render a "dummy" box of the correct size so that it occupies
        // the proper amount of space
        ImGui.SetCursorScreenPos(min);
        ImGui.Dummy(max - min);
    }
}