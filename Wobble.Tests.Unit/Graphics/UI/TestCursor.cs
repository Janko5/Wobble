using Wobble.Graphics.UI;
using Xunit;

namespace Wobble.Tests.Unit.Graphics.UI
{
    public class TestCursor
    {
        [Fact]
        public void SizeScaleResizesCursorFromOriginalSize()
        {
            var cursor = new Cursor(null, 40)
            {
                SizeScale = 1.5f
            };

            Assert.Equal(60, cursor.Width);
            Assert.Equal(60, cursor.Height);
        }

        [Fact]
        public void SizeScaleClampsToMinimumScale()
        {
            var cursor = new Cursor(null, 40)
            {
                SizeScale = 0
            };

            Assert.Equal(0.1f, cursor.SizeScale);
            Assert.Equal(4, cursor.Width);
            Assert.Equal(4, cursor.Height);
        }

        [Fact]
        public void SizeScaleClampsToMaximumScale()
        {
            var cursor = new Cursor(null, 40)
            {
                SizeScale = 3
            };

            Assert.Equal(2, cursor.SizeScale);
            Assert.Equal(80, cursor.Width);
            Assert.Equal(80, cursor.Height);
        }
    }
}
