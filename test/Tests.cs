using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TownBuilderBot
{
    public static class TestExtensionMethods
    {
        public static string ReplaceElement(string grid, int width, int x, int y, string newString)
        {
            int index = y * (width + 1) + x;

            string prefix = grid.Substring(0, index);

            string suffix = grid.Substring(index + 1);

            return prefix + newString + suffix;
        }

        [Fact]
        public static void ReplaceElement_Test()
        {
            string grid = "ABCD\n"
                        + "EFGH\n"
                        + "IJKL\n"
                        + "MNOP";

            string result = ReplaceElement(grid, 4, 1, 2, "X");

            string expectedResult = "ABCD\n"
                                  + "EFGH\n"
                                  + "IXKL\n"
                                  + "MNOP";

            Assert.Equal(result, expectedResult);
        }
    }
}