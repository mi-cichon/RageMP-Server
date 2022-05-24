using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    public class CharacterCreator
    {
        public int[] fathers = new int[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 42, 43, 44
        };
        public int[] mothers = new int[]
        {
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 45
        };
        public int[] maleHair = new int[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 ,24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 72, 73
        };
        public int[] femaleHair = new int[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 ,24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 76, 77
        };

        public int maxHairColors = 64;
        public int maxBlushColors = 27;
        public int maxEyeColors = 32;
        public int maxLipsticColors = 32;

        public CharacterCreator()
        {

        }
    }
}
