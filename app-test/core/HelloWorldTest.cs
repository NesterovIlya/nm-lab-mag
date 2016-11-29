using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using app.core;

namespace app_test.core
{
    [TestClass]
    public class HelloWorldTest
    {

        [TestMethod]
        public void Test_SayHello()
        {
            HelloWorld helloWorld = new HelloWorld();
            Assert.AreEqual("Hello!", helloWorld.SayHello());
        }


    }
}
