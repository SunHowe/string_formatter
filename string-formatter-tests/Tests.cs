using System;
using NUnit.Framework;
using StringFormatter;

namespace string_formatter_tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SimpleFormatTest()
        {
            Assert.AreEqual(StringEx.Format("{0}", "param"), "param");
        }

        [Test]
        public void RuleFormatTest()
        {
            StringEx.RegisterFormatFunc("name", () => "玩家A");
            Assert.AreEqual(StringEx.Format("{name}"), "玩家A");
        }

        [Test]
        public void RuleFormatWithParametersTest()
        {
            StringEx.RegisterFormatFunc("i18n", (index, parameters) => $"文本内容({parameters[index]})");
            
            Assert.AreEqual(StringEx.Format("{0:i18n}", "key_1001"), "文本内容(key_1001)");
        }

        [Test]
        public void SpecialCharTest()
        {
            Assert.AreEqual(StringEx.Format("{0}{{0}}", "param"), "param{0}");
        }

        [Test]
        public void ComplexFormatTest()
        {
            StringEx.RegisterFormatFunc("name", () => "玩家A");
            StringEx.RegisterFormatFunc("i18n", (index, parameters) => $"文本内容({parameters[index]})");
            
            Assert.AreEqual(StringEx.Format("{name}说了一句:\"{1:i18n}\", 手上拿着{0}", "一把刀", "key_1001"), 
                "玩家A说了一句:\"文本内容(key_1001)\", 手上拿着一把刀");
        }
    }
}