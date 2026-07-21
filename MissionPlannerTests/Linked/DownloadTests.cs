using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class DownloadTests
    {
        [TestMethod()]
        public void getFilefromNetTest()
        {
            var payload = Encoding.UTF8.GetBytes("response without a content-length header");
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint) listener.LocalEndpoint).Port;
            var server = Task.Run(() =>
            {
                using (var connection = listener.AcceptTcpClient())
                using (var stream = connection.GetStream())
                using (var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true))
                {
                    while (!String.IsNullOrEmpty(reader.ReadLine()))
                    {
                    }

                    var headers = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nConnection: close\r\n\r\n");
                    stream.Write(headers, 0, headers.Length);
                    stream.Write(payload, 0, payload.Length);
                }
            });

            var destination = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            try
            {
                Assert.IsTrue(Utilities.Download.getFilefromNet("http://127.0.0.1:" + port + "/", destination));
                CollectionAssert.AreEqual(payload, File.ReadAllBytes(destination));
                Assert.IsTrue(server.Wait(TimeSpan.FromSeconds(5)));
            }
            finally
            {
                listener.Stop();
                if (File.Exists(destination))
                    File.Delete(destination);
                if (File.Exists(destination + ".new"))
                    File.Delete(destination + ".new");
            }
        }

        [TestMethod()]
        public void CheckHTTPFileExists()
        {
            if (Utilities.Download.CheckHTTPFileExists("https://github.com/ArduPilot/MissionPlanner/releases/download/betarelease/MissionPlannerBeta.zip"))
                return;

            Assert.Fail();
        }

        [TestMethod()]
        public void GetFileSize()
        {
            if (Utilities.Download.GetFileSize("https://github.com/ArduPilot/MissionPlanner/releases/download/betarelease/MissionPlannerBeta.zip") > 0)
                return;

            Assert.Fail();
        }
    }
}
