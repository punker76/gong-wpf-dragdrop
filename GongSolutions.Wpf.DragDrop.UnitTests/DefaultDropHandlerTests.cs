using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GongSolutions.Wpf.DragDrop.UnitTests
{
    [TestClass]
    public class DefaultDropHandlerTests
    {
        [TestMethod]
        public void TestCompatibleTypes_Of_Same_Type()
        {
            Assert.IsTrue(DefaultDropHandler_Accessor.TestCompatibleTypes(
                                                                          new List<string>(),
                                                                          new[] {"Foo", "Bar"}));
        }

        [TestMethod]
        public void TestCompatibleTypes_Common_Interface()
        {
            Assert.IsTrue(DefaultDropHandler_Accessor.TestCompatibleTypes(
                                                                          new List<IInterface>(),
                                                                          new[] {new BaseClass(), new DerivedClassA()}));
        }

        [TestMethod]
        public void TestCompatibleTypes_Collection_TooDerived()
        {
            Assert.IsFalse(DefaultDropHandler_Accessor.TestCompatibleTypes(
                                                                           new List<DerivedClassA>(),
                                                                           new[] {new BaseClass(), new DerivedClassA()}));
        }

        private interface IInterface
        {
        }

        private class BaseClass : IInterface
        {
        }

        private class DerivedClassA : BaseClass
        {
        }

        private class DerivedClassB : BaseClass
        {
        }
    }
}