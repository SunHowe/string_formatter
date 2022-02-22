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
             Assert.AreEqual(StringEx.Format("{0}{1}{0}{{}}", "param1", "param2"), "param1param2param1{}");
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

        [Test]
        public void NestedFormatTest()
        {
            StringEx.RegisterFormatFunc("name", () => "玩家A");
            StringEx.RegisterFormatFunc("i18n", (index, parameters) => $"文本内容({parameters[index]})");
            StringEx.RegisterFormatFunc("format", (index, parameters) =>
            {
                var format = parameters[index] as string;

                var size = parameters.Length - index - 1;
                if (size <= 0)
                    return StringEx.Format(format);
                
                var newParameters = new object[size];
                Array.Copy(parameters, index + 1, newParameters, 0, size);

                return StringEx.Format(format, newParameters);
            });
            
            Assert.AreEqual(StringEx.Format("{name}说了一句:\"{0:format}\"", "{name},{0:i18n}", "key_1001"), 
                "玩家A说了一句:\"玩家A,文本内容(key_1001)\"");
        }

        [Test]
        public void ErrorTest1()
        {
            Assert.Catch<FormatException>(() =>
            {
                StringEx.Format("}");
            });
        }

        [Test]
        public void ErrorTest2()
        {
            Assert.Catch<FormatException>(() =>
            {
                StringEx.Format("{");
            });
        }

        [Test]
        public void ErrorTest3()
        {
            Assert.Catch<FormatException>(() =>
            {
                StringEx.Format("{}");
            });
        }

        [Test]
        public void ErrorTestWithParametersCountError()
        {
            Assert.Catch<FormatException>(() =>
            {
                StringEx.Format("{1}");
            });
        }

        [Test]
        public void DifferentContextTest()
        {
            var formatter1 = new StringFormatter.StringFormatter();
            formatter1.RegisterFormatFunc("name", () => "name in context1");
            
            var formatter2 = new StringFormatter.StringFormatter();
            formatter2.RegisterFormatFunc("name", () => "name in context2");
            
            Assert.AreEqual(formatter1.Format("{name}"), "name in context1");
            Assert.AreEqual(formatter2.Format("{name}"), "name in context2");
        }
    }
}