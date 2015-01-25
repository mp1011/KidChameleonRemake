using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class GameFont
    {
        private RGPointI upperLeft;
        private RGSizeI cellSize;
        private Dictionary<char, RGPointI> letterLocations;

        public RGSizeI CellSize { get { return cellSize; } }
        public TextureResource FontTexture { get; private set; }

        public GameFont(TextureResource fontTexture, RGSizeI p_cellSize, RGPointI p_upperLeft)
        {
            cellSize = p_cellSize;
            upperLeft = p_upperLeft;
            letterLocations = new Dictionary<char, RGPointI>();
            this.FontTexture = fontTexture;
        }

        public void AddAlphabet(RGRectangleI area)
        {
            int x = area.X * cellSize.Width, y = area.Y * cellSize.Height;
            char curChar = 'A';

            while (curChar <= 'Z')
            {
                letterLocations.Add(curChar, new RGPointI(x, y));
                x += cellSize.Width;

                curChar++;
                if (x >= (area.Width + 1) * cellSize.Width)
                {
                    x = area.X * cellSize.Width;
                    y += cellSize.Height;

                    if (y >= (area.Height + 1) * cellSize.Height)
                        return;
                }
            }
        }

        public void AddCharacter(char character, RGPointI location)
        {
            letterLocations.Add(character, new RGPointI(location.X * cellSize.Width, location.Y * cellSize.Height));
        }

        public void AddCharacters(String characters, RGPointI location)
        {
            int x = (int)(location.X * cellSize.Width), y = (int)(location.Y * cellSize.Height);
            char[] chars = characters.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                letterLocations.Add(chars[i], new RGPointI(x, y));
                x += cellSize.Width;
            }
        }

        public RGRectangleI GetLetterTextureLocation(char character)
        {
            if (letterLocations.ContainsKey(character))
                return RGRectangleI.FromXYWH(letterLocations[character].X, letterLocations[character].Y, cellSize.Width, cellSize.Height);
            else
                return RGRectangleI.Empty;
        }
    }

    public class Letter
    {
        private ulong activeFrame;

        public char character;
        public RGPointI location;
        public RGColor color;
    }

}
