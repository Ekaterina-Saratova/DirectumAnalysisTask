using ConsoleUI;
using System.Xml;

namespace Tests
{
    public class Tests
    {
        private const string FolderName = "xmlFiles";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WellFormedXmlFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName, "WellFormed.xml");
            Assert.That(TwoVersions.GetXmlAttributeValueByElementAndName(path, "book", "case"), Is.EqualTo("1"));
        }

        [Test]
        public void WellFormedXmlFileNoAttribute()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName, "WellFormed.xml");
            Assert.That(TwoVersions.GetXmlAttributeValueByElementAndName(path, "book", "case1"), Is.Null);
        }

        [Test]
        public void WelFormedXmlFileOneLine()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName, "WellFormedOneLine.xml");
            Assert.That(TwoVersions.GetXmlAttributeValueByElementAndName(path, "book", "case"), Is.EqualTo("1"));
        }

        [Test]
        public void WelFormedXmlChaoticLinebreaks()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName, "WellFormedXmlChaoticLinebreaks.xml");
            Assert.That(TwoVersions.GetXmlAttributeValueByElementAndName(path, "book", "case"), Is.EqualTo("1     "));
        }

        [Test]
        public void NotWellFormedXmlFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName, "NotWellFormed.xml");
            Assert.Throws<XmlException>(() => TwoVersions.GetXmlAttributeValueByElementAndName(path, "book", "case"));
        }
    }
}