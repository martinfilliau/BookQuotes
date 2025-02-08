using System.Xml;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Tests;

public class XmlAnnotationsParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Test_Null_Empty_Xml(string value)
    {
        var result = XmlAnnotationsParser.Parse(value);
        Assert.Null(result);
    }

    [Fact]
    public void Test_Not_Xml()
    {
        // XXX TODO should have same behavior than bad XML
        Assert.Throws<XmlException>(() => XmlAnnotationsParser.Parse("boo"));
    }

    [Fact]
    public void Test_Bad_Xml()
    {
        // XXX TODO should have same behavior -- or null?
        const string input = @"<tag1 att1 = ""test"">
                                <tag2><!--Test comment-->Test</tag2>
                                <tag2>Test 2</tag2>
                             </tag1>";
        
        Assert.Throws<Exception>(() => XmlAnnotationsParser.Parse(input));
    }

    [Fact]
    public void Test_Xml_No_Quotes()
    {
        const string input =
            @"<annotationSet xmlns:xhtml=""http://www.w3.org/1999/xhtml"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns=""http://ns.adobe.com/digitaleditions/annotations"">
                <publication>
                    <dc:title>Samouraï</dc:title>
                    <dc:creator>Fabrice Caro</dc:creator>
                    <dc:description>SFC</dc:description>
                    <dc:language>fr</dc:language>
                </publication>
            </annotationSet>";

        var result = XmlAnnotationsParser.Parse(input);

        Assert.NotNull(result);
        Assert.Equal("Samouraï", result.Title);
        Assert.Equal("Fabrice Caro", result.Author);
        Assert.Empty(result.Quotes);
    }

    [Fact]
    public void Test_Xml_With_Quotes()
    {
        const string input =
            @"<annotationSet xmlns:xhtml=""http://www.w3.org/1999/xhtml"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns=""http://ns.adobe.com/digitaleditions/annotations"">
        <publication>
            <dc:identifier>amnt</dc:identifier>
            <dc:title>Architecture Modernization: Socio-technical alignment of software, strategy, and structure</dc:title>
            <dc:creator>Nick Tune</dc:creator>
            <dc:language>en</dc:language>
        </publication>
        <annotation>
            <dc:identifier>urn:uuid:497c97ff-efea-440d-bc24-2715f8043c27</dc:identifier>
            <dc:date>2024-08-07T13:15:32Z</dc:date>
            <dc:creator>urn:uuid:156adb1c-9d3a-4a2d-830a-63a9831c5a7c</dc:creator>
            <target>
                <fragment start=""OEBPS/Text/07.htm#point(/1/4/184/8:228)"" end=""OEBPS/Text/07.htm#point(/1/4/185:1)"" progress=""0.311828"" color=""4"">
                    <text>One way to check if you have suitable pivotal events is to ask: Do the pivotal events alone tell the high-level story of the domain?
        </text>
                </fragment>
            </target>
        </annotation>
        <annotation>
            <dc:identifier>urn:uuid:7fbad2bc-6a03-4e31-9ced-b1a6a39d4f0f</dc:identifier>
            <dc:date>2024-08-09T07:58:08Z</dc:date>
            <dc:creator>urn:uuid:156adb1c-9d3a-4a2d-830a-63a9831c5a7c</dc:creator>
            <target>
                <fragment start=""OEBPS/Text/08.htm#point(/1/4/91:4)"" end=""OEBPS/Text/08.htm#point(/1/4/92/2:154)"" progress=""0.35914"" color=""4"">
                    <text>Sometimes, engineers don’t talk to users because of cultural perspectives. For instance, their only value is perceived as sitting at their desks coding.</text>
                </fragment>
            </target>
        </annotation>
    </annotationSet>";

        var result = XmlAnnotationsParser.Parse(input);

        Assert.NotNull(result);
        Assert.Equal("Architecture Modernization: Socio-technical alignment of software, strategy, and structure",
            result.Title);
        Assert.Equal("Nick Tune", result.Author);
        Assert.Equal(2, result.Quotes.Count);
        var firstQuote = result.Quotes[0];
        Assert.Equal("OEBPS/Text/07.htm#point(/1/4/184/8:228)", firstQuote.Position);
        Assert.Equal("One way to check if you have suitable pivotal events is to ask: Do the pivotal events alone tell the high-level story of the domain?",
            firstQuote.Comment);
    }
}