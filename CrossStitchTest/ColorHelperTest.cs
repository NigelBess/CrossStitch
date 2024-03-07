using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using CrossStitch;
using System.Linq;

namespace CrossStitchTest
{
    [TestClass]
    public class ColorHelperTest
    {
        [TestMethod]
        public void TestConversion()
        {
            var color = Color.FromArgb(255,61,235,52);
            var hsv = ColorHelper.ColorToHSV(color);
            var expectedHSV = new float[]{117f, .779f, .922f };
            foreach(var (actual,expected) in hsv.Zip(expectedHSV))
            {
                Assert.AreEqual(actual, expected,.1f);
            }

            var newColor = ColorHelper.HSVToColor(hsv);
            Assert.AreEqual(color, newColor);
        }
    }
}
