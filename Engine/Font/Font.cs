using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface IGameFont
    {
        TextureResource FontTexture { get; }
        RGRectangleI GetLetterTextureLocation(char character);      
    }

    public static class IGameFontUtil
    {
        public static RGSizeI LetterSize(this IGameFont font, char c)
        {
            return font.GetLetterTextureLocation(c).Size;
        }

        public static int LetterWidth(this IGameFont font, char c)
        {
            return font.GetLetterTextureLocation(c).Width;
        }

        public static int LetterHeight(this IGameFont font, char c)
        {
            return font.GetLetterTextureLocation(c).Height;
        }

        public static RGSizeI SpaceSize(this IGameFont font)
        {
            return font.LetterSize(' ');
        }

        //temp
        public static RGSizeI CellSize(this IGameFont font)
        {
            return font.LetterSize(' ');
        }
    }

    public class FixedSpaceFont : IGameFont
    {
        private RGPointI upperLeft;
        private RGSizeI cellSize;
        private Dictionary<char, RGPointI> letterLocations;

        public RGSizeI CellSize { get { return cellSize; } }
        public TextureResource FontTexture { get; private set; }

        public FixedSpaceFont(TextureResource fontTexture, RGSizeI p_cellSize, RGPointI p_upperLeft)
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


    public class VariableSpaceFont : IGameFont
    {
        private Dictionary<char, RGRectangleI> mLetterLocations;

        public VariableSpaceFont(TextureResource fontTexture)
        {           
            mLetterLocations = new Dictionary<char, RGRectangleI>();
            this.FontTexture = fontTexture;
        }

        public void AddRow(int top, int bottom, string letters, params int[] splits)
        {
            int index = 0;

            foreach (char c in letters)
            {
                AddLetter(c, RGRectangleI.FromTLBR(top, splits[index], bottom, splits[index + 1]));
                index += 1;
            }          

        }

        public void AddLetter(char c, RGRectangleI location)
        {
            mLetterLocations.Add(c, location);
        }

        public TextureResource FontTexture
        {
            get; private set;
        }

        public RGRectangleI GetLetterTextureLocation(char character)
        {
            if (mLetterLocations.ContainsKey(character))
                return mLetterLocations[character];
            else
                return RGRectangleI.Empty;
        }
    }

    public class Letter
    {
        public char Character;
        public RGRectangleI Location;
        public RGColor Color;
    }

}
