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
        public void TestRGB()
        {
            var color = Color.FromArgb(255,61,235,52);
            var rgb = new[] { .239f, .921f, .203f};
            TestConverter(new RGBConverter(),color,rgb);
        }

        [TestMethod]
        public void TestHSV()
        {
            var color = Color.FromArgb(255, 61, 235, 52);
            var hsv = new []{.325f,.779f,.922f };
            TestConverter(new HSVConverter(),color,hsv);
        }

        private void TestConverter(IColorConverter converter, Color color, float[] raw)
        {
            var hsv = converter.ToRaw(color);
            foreach (var (actual, expected) in hsv.Zip(raw))
            {
                Assert.AreEqual(expected, actual, .1f);
            }

            var newColor = converter.FromRaw(hsv);
            Assert.AreEqual(color, newColor);
        }
    }
}
