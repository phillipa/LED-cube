using System;
using System.Drawing;

namespace WonderWall
{
    /// <summary>
    /// Class with useful palette functions
    /// </summary>
    public class Palette
    {
        /// <summary>
        /// Predefined palettes. Each is an array of 16 colors that can
        /// be used to generate palettes of arbitrary lengths using functions
        /// in this class.
        /// </summary>
        public Color[] adbasic; //blues, purples, some green
        public Color[] purplegreen;
        public Color[] greenpurple;
        public Color[] darkpurples;
        public Color[] bluespurples;
        public Color[] blues;
        public Color[] greens;

        Color bluejay = Color.FromArgb(0x2B547E);
        Color navyblue = Color.FromArgb(0x000080);
        Color lapisblue = Color.FromArgb(0x15317E);
        Color earthblue = Color.FromArgb(0x0000A0);
        Color cobaltblue = Color.FromArgb(0x0020C2);
        Color blueorchid = Color.FromArgb(0x1F45FC);


        public Palette()
        {
            InitPalettes();
        }

        private void InitPalettes()
        {
            adbasic = new Color[]{ Color.Purple, Color.Purple, Color.Purple, Color.Purple,
                    Color.Blue, Color.Blue, Color.Blue, Color.Chartreuse,
                    Color.Green, Color.Purple, Color.Purple, Color.Purple,
                    Color.Blue, Color.Blue, Color.DarkMagenta, Color.DarkMagenta};

            purplegreen = new Color[] {Color.DarkMagenta, Color.DarkOrchid, Color.DarkViolet, Color.MediumOrchid,
            Color.MediumPurple, Color.Purple, Color.BlueViolet, Color.Blue,
            Color.Blue, Color.Blue, Color.Chartreuse, Color.Green,
            Color.Chartreuse, Color.YellowGreen, Color.Lime, Color.Chartreuse};

            greenpurple = new Color[] {Color.Chartreuse, Color.Lime, Color.YellowGreen, Color.Chartreuse,
            Color.Green, Color.Chartreuse, Color.Blue, Color.Blue,
            Color.Blue, Color.BlueViolet, Color.Purple, Color.MediumPurple,
            Color.MediumPurple, Color.DarkViolet, Color.DarkOrchid, Color.DarkMagenta};


            darkpurples = new Color[] {Color.DarkMagenta, Color.DarkMagenta, Color.DarkOrchid, Color.DarkOrchid,
            Color.DarkViolet, Color.DarkViolet, Color.BlueViolet, Color.BlueViolet,
            Color.DarkMagenta, Color.DarkMagenta, Color.DarkOrchid, Color.DarkOrchid,
            Color.DarkViolet, Color.DarkViolet, Color.BlueViolet, Color.BlueViolet};

            greens = new Color[] {Color.LimeGreen, Color.LimeGreen, Color.Lime, Color.Lime,
            Color.Green ,Color.Green, Color.Chartreuse, Color.Chartreuse,
            Color.YellowGreen, Color.YellowGreen, Color.Chartreuse, Color.Chartreuse,
            Color.LimeGreen, Color.LimeGreen, Color.Lime, Color.Lime};

            blues = new Color[] {Color.Blue, Color.Blue, bluejay, bluejay,
            navyblue, navyblue, lapisblue, lapisblue,
            Color.CornflowerBlue, Color.CornflowerBlue, earthblue, earthblue,
            cobaltblue, cobaltblue, blueorchid, blueorchid};


    
        }


    }
}
