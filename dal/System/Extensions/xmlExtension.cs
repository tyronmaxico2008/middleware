using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class xmlExtension
    {
        public static string getXmlText(this System.Xml.XmlNode node, string xPath="")
        {
            var node1 = xPath.isEmpty()? node : node.SelectSingleNode(xPath);

            if (node1 == null)
                return "";
            else
                return node1.InnerText;
        }

        public static string getXmlAttributeValue(this System.Xml.XmlNode node, string sAttriName)
        {
            if (node == null) return "";
            if (node.Attributes[sAttriName] == null) return "";
            return node.Attributes[sAttriName].InnerText;
        }

    }
}
