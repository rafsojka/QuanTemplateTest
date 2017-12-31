using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace QuanTemplateTest.EndToEndTests
{
    [TestFixture]
    public class QuanTemplateTestEndToEndTests
    {
        [Test, TestCaseSource("FileInputTestCaseSource")]
        public void Project_EndToEndTest(string projectNameString, string fileName, string tickSize, string bookDepth, string outputString)
        {
            var p = GetProcess(projectNameString, fileName, tickSize, bookDepth);

            p.Start();

            StreamWriter myStreamWriter = p.StandardInput;

            var output = p.StandardOutput.ReadToEnd();
            p.StandardOutput.Close();

            myStreamWriter.WriteLine();

            p.WaitForExit();

            Assert.That(output, Is.EqualTo(outputString));
        }

        private Process GetProcess(string projectName, string fileName, string tickSize, string bookDepth)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.FileName = $@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\..\..\..\{projectName}\bin\Debug\{projectName}.exe";
            p.StartInfo.Arguments = $"{fileName} {tickSize} {bookDepth}";

            return p;
        }

        private static IEnumerable<TestCaseData> FileInputTestCaseSource
        {
            get
            {
                yield return new TestCaseData("QuanTemplateTest", getTestFilePath("updates.txt"), "10.0", "2", getTextFromFile("QuanTemplateTest_Output_1.txt"));
                yield return new TestCaseData("QuanTemplateTest", getTestFilePath("updates2.txt"), "10.0", "5", getTextFromFile("QuanTemplateTest_Output_2.txt"));
                yield return new TestCaseData("QuanTemplateTest", getTestFilePath("updates3.txt"), "10.0", "5", getTextFromFile("QuanTemplateTest_Output_3.txt"));

                //yield return new TestCaseData("QuanTemplateTest", "\"some not existing file.csv\"", "1000", getTextFromFile("ZopaTest_Output_2.txt"));
                //yield return new TestCaseData("ZopaTest", "\"..\\..\\market_offers\\Market Data for Exercise.csv\"", "800", getTextFromFile("ZopaTest_Output_3.txt"));
                //yield return new TestCaseData("ZopaTest", "\"..\\..\\market_offers\\Market Data for Exercise.csv\"", "30000", getTextFromFile("ZopaTest_Output_4.txt"));
                //yield return new TestCaseData("ZopaTest", "\"..\\..\\market_offers\\Market Data for Exercise.csv\"", "8333", getTextFromFile("ZopaTest_Output_5.txt"));
                //yield return new TestCaseData("ZopaTest", "\"..\\..\\market_offers\\Market Data for Exercise.csv\"", "8000", getTextFromFile("ZopaTest_Output_6.txt"));
            }
        }

        private static string getTextFromFile(string fileName)
        {
            return string.Concat(System.IO.File.ReadAllText(getTestFilePath(fileName)), "\r\n");
        }

        private static string getTestFilePath(string fileName)
        {
            return $@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\..\..\TestCaseSources\{fileName}";
        }

    }
}
