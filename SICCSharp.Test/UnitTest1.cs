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
            var wordOffset = SICCSharp.CompileHelper.CalculateLocationOffset("WORD");
            Assert.Equal(3, wordOffset);
            var LDXOffset = CompileHelper.CalculateLocationOffset("LDX");
            Assert.Equal(3, LDXOffset);
            var RESWOffet = CompileHelper.CalculateLocationOffset("RESW", 100);
            Assert.Equal(300, RESWOffet);
            var RESBOffset = CompileHelper.CalculateLocationOffset("RESB", 100);
            Assert.Equal(100, RESBOffset);
        }
    }
}
