using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassMapper
{
    [TestClass]
    public class ClassMapperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var foo = new Foo { Prop1 = "1", Prop2 = "1" };
            var bar = mapper.Map(foo);
            Assert.AreEqual(foo.Prop1, bar.Prop1);
            Assert.AreEqual(bar.Prop2, null);
        }
    }
}
