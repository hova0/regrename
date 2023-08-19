using regrename;

namespace regrenameTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConsoleOptionsTest()
        {
            ConsoleOptions co = new ConsoleOptions(new string[] { "--arg", "value", "value", "-arg", "value", "-arg:value", "--arg=value", "value"} );
            Assert.IsTrue(co[0].key == "arg");
            Assert.IsTrue(co[0].value == "value");
            Assert.IsTrue(co[1].key == null);
            Assert.IsTrue(co[1].value == "value");
            Assert.IsTrue(co[1].ordinal == 0);
            Assert.IsTrue(co[2].key == "arg");
            Assert.IsTrue(co[2].value == "value");
            Assert.IsTrue(co[3].key == "arg");
            Assert.IsTrue(co[3].value == "value");
            Assert.IsTrue(co[4].ordinal == 1);
            Assert.IsTrue(co[4].value == "value");


        }

        [TestMethod]
        public void RealTest()
        {
            ConsoleOptions co = new ConsoleOptions(new string[] { "\\\\hovastorage\\MainStorage\\General\\Anime", "\\[ANBU\\].*", "$0", "--preview" });
            Assert.IsTrue(co[0].ordinal == 0);
            Assert.IsTrue(co.Count == 4, "Does not contain 4 items");
        }
    }
}