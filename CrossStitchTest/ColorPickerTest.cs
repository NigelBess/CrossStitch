using System;
using System.Collections.Generic;
using System.Text;
using CrossStitch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossStitchTest
{
    [TestClass]
    public class ColorPickerTest
    {
        [TestMethod]
        public void Test3D()
        {
            const int numberOfShips = 1000;
            const int numberOfSatellites = 8;
            const int dimensions = 3;

            var ships = ColorPicker.GeneratePoints(numberOfShips, dimensions);
            var satellites = ColorPicker.PickColors(ships, numberOfSatellites, 1000, 1e9f);
        }

        
    }
}
