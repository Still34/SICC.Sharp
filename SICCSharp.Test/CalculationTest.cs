using SICCSharp.Extensions;
using Xunit;

namespace SICCSharp.Test
{
    public class CalculationTest
    {
        [Fact]
        public void AddHex_0008AB_000B78_Expect_005155()
        {
            var add = MathHelper.AddHex("0008AB", "000B78");
            Assert.Equal("1423", add);
        }

        [Fact]
        public void BitArrayCalc_1234_Expect_0001001000111101()
        {
            var bitArray = MathHelper.CalculateOpCodeBitArray("123D");
            Assert.Equal("0001001000111101", bitArray);
        }

        [Fact]
        public void SubHex_000250_000128_Expect_000128()
        {
            var sub = MathHelper.SubHex("000250", "000128");
            Assert.Equal("128", sub);
        }
    }
}