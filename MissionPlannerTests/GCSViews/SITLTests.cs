using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace MissionPlanner.GCSViews.Tests
{
    [TestClass]
    public class SITLTests
    {
        [TestMethod]
        public void MacOSBinaryDetectionTest()
        {
            var isMachO = typeof(SITL).GetMethod("IsMachO", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(isMachO);

            var machOPath = Path.GetTempFileName();
            var windowsPath = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(machOPath, new byte[] {0xcf, 0xfa, 0xed, 0xfe, 0, 0, 0, 0});
                File.WriteAllBytes(windowsPath, new byte[] {0x4d, 0x5a, 0x90, 0, 0, 0, 0, 0});

                Assert.IsTrue((bool) isMachO.Invoke(null, new object[] {machOPath}));
                Assert.IsFalse((bool) isMachO.Invoke(null, new object[] {windowsPath}));
            }
            finally
            {
                File.Delete(machOPath);
                File.Delete(windowsPath);
            }
        }
    }
}
