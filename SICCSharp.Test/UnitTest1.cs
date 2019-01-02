using System;
using SICCSharp;
using Xunit;

namespace SICCSharp.Test
{
    public class UnitTest1
    {
        [Fact]
        public void CalculateLocationOffset()
        {
            var wordOffset = SICCSharp.MathHelper.CalculateLocationOffset("WORD");
            Assert.Equal(3, wordOffset);
            var LDXOffset = MathHelper.CalculateLocationOffset("LDX");
            Assert.Equal(3, LDXOffset);
            var RESWOffet = MathHelper.CalculateLocationOffset("RESW", 100);
            Assert.Equal(300, RESWOffet);
            var RESBOffset = MathHelper.CalculateLocationOffset("RESB", 100);
            Assert.Equal(100, RESBOffset);
        }
    }
}
