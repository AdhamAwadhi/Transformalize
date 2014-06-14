﻿// http://www.codeproject.com/Tips/682245/NanoXML-Simple-and-fast-XML-parser

using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Extensions;

namespace Transformalize.Libs {
    /// <summary>
    ///     Base class containing useful features for all XML classes
    /// </summary>
    internal class NanoXmlBase {
        protected static bool IsSpace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        protected static void SkipSpaces(string str, ref int i) {
            while (i < str.Length) {
                if (!IsSpace(str[i])) {
                    if (str[i] == '<' && i + 4 < str.Length && str[i + 1] == '!' && str[i + 2] == '-' && str[i + 3] == '-') {
                        i += 4; // skip <!--

                        while (i + 2 < str.Length && !(str[i] == '-' && str[i + 1] == '-'))
                            i++;

                        i += 2; // skip --
                    } else
                        break;
                }

                i++;
            }
        }

        protected static string GetValue(string str, ref int i, char endChar, char endChar2, bool stopOnSpace) {
            int start = i;
            while ((!stopOnSpace || !IsSpace(str[i])) && str[i] != endChar && str[i] != endChar2)
                i++;

            return str.Substring(start, i - start);
        }

        protected static bool IsQuote(char c) {
            return c == '"' || c == '\'';
        }

        // returns name
        protected static string ParseAttributes(string str, ref int i, List<NanoXmlAttribute> attributes, char endChar, char endChar2) {
            SkipSpaces(str, ref i);
            string name = GetValue(str, ref i, endChar, endChar2, true);

            SkipSpaces(str, ref i);

            while (str[i] != endChar && str[i] != endChar2) {
                string attrName = GetValue(str, ref i, '=', '\0', true);

                SkipSpaces(str, ref i);
                i++; // skip '='
                SkipSpaces(str, ref i);

                char quote = str[i];
                if (!IsQuote(quote))
                    throw new XmlParsingException("Unexpected token after " + attrName);

                i++; // skip quote
                string attrValue = GetValue(str, ref i, quote, '\0', false);
                i++; // skip quote

                attributes.Add(new NanoXmlAttribute(attrName, attrValue));

                SkipSpaces(str, ref i);
            }

            return name;
        }
    }

    /// <summary>
    ///     Class representing whole DOM XML document
    /// </summary>
    internal class NanoXmlDocument : NanoXmlBase {
        private readonly List<NanoXmlAttribute> declarations = new List<NanoXmlAttribute>();
        private readonly NanoXmlNode rootNode;

        /// <summary>
        ///     Public constructor. Loads xml document from raw string
        /// </summary>
        /// <param name="xmlString">String with xml</param>
        public NanoXmlDocument(string xmlString) {
            int i = 0;

            while (true) {
                SkipSpaces(xmlString, ref i);

                if (xmlString[i] != '<')
                    throw new XmlParsingException("Unexpected token");

                i++; // skip <

                if (xmlString[i] == '?') // declaration
                {
                    i++; // skip ?
                    ParseAttributes(xmlString, ref i, declarations, '?', '>');
                    i++; // skip ending ?
                    i++; // skip ending >

                    continue;
                }

                if (xmlString[i] == '!') // doctype
                {
                    while (xmlString[i] != '>') // skip doctype
                        i++;

                    i++; // skip >

                    continue;
                }

                rootNode = new NanoXmlNode(xmlString, ref i);
                break;
            }
        }

        /// <summary>
        ///     Root document element
        /// </summary>
        public NanoXmlNode RootNode {
            get { return rootNode; }
        }

        /// <summary>
        ///     List of XML Declarations as <see cref="NanoXmlAttribute" />
        /// </summary>
        public IEnumerable<NanoXmlAttribute> Declarations {
            get { return declarations; }
        }
    }

    /// <summary>
    ///     Element node of document
    /// </summary>
    internal class NanoXmlNode : NanoXmlBase {
        private readonly List<NanoXmlAttribute> attributes = new List<NanoXmlAttribute>();
        private readonly string name;

        private readonly List<NanoXmlNode> subNodes = new List<NanoXmlNode>();
        private readonly string value;

        internal NanoXmlNode(string str, ref int i) {
            name = ParseAttributes(str, ref i, attributes, '>', '/');

            if (str[i] == '/') // if this node has nothing inside
            {
                i++; // skip /
                i++; // skip >
                return;
            }

            i++; // skip >

            // temporary. to include all whitespaces into value, if any
            int tempI = i;

            SkipSpaces(str, ref tempI);

            if (str[tempI] == '<') {
                i = tempI;

                while (str[i + 1] != '/') // parse subnodes
                {
                    i++; // skip <
                    subNodes.Add(new NanoXmlNode(str, ref i));

                    SkipSpaces(str, ref i);

                    if (i >= str.Length)
                        return; // EOF

                    if (str[i] != '<')
                        throw new XmlParsingException("Unexpected token");
                }

                i++; // skip <
            } else // parse value
            {
                value = GetValue(str, ref i, '<', '\0', false);
                i++; // skip <

                if (str[i] != '/')
                    throw new XmlParsingException("Invalid ending on tag " + name);
            }

            i++; // skip /
            SkipSpaces(str, ref i);

            string endName = GetValue(str, ref i, '>', '\0', true);
            if (endName != name)
                throw new XmlParsingException("Start/end tag name mismatch: " + name + " and " + endName);
            SkipSpaces(str, ref i);

            if (str[i] != '>')
                throw new XmlParsingException("Invalid ending on tag " + name);

            i++; // skip >
        }

        /// <summary>
        ///     Element value
        /// </summary>
        public string Value {
            get { return value; }
        }

        /// <summary>
        ///     Element name
        /// </summary>
        public string Name {
            get { return name; }
        }

        /// <summary>
        ///     List of subelements
        /// </summary>
        public IEnumerable<NanoXmlNode> SubNodes {
            get { return subNodes; }
        }

        /// <summary>
        ///     List of attributes
        /// </summary>
        public IEnumerable<NanoXmlAttribute> Attributes {
            get { return attributes; }
        }

        /// <summary>
        ///     Returns subelement by given name
        /// </summary>
        /// <param name="nodeName">Name of subelement to get</param>
        /// <returns>First subelement with given name or NULL if no such element</returns>
        public NanoXmlNode this[string nodeName] {
            get {
                foreach (NanoXmlNode nanoXmlNode in subNodes)
                    if (nanoXmlNode.name == nodeName)
                        return nanoXmlNode;

                return null;
            }
        }

        /// <summary>
        ///     Returns attribute by given name
        /// </summary>
        /// <param name="attributeName">Attribute name to get</param>
        /// <returns><see cref="NanoXmlAttribute" /> with given name or null if no such attribute</returns>
        public NanoXmlAttribute GetAttribute(string attributeName) {
            foreach (NanoXmlAttribute nanoXmlAttribute in attributes)
                if (nanoXmlAttribute.Name == attributeName)
                    return nanoXmlAttribute;

            return null;
        }

        public string InnerText() {
            var builder = new StringBuilder();
            InnerText(ref builder);
            return builder.ToString();
        }

        private void InnerText(ref StringBuilder builder) {
            foreach (var node in subNodes) {
                builder.Append("<");
                builder.Append(node.Name);
                foreach (var attribute in node.attributes) {
                    builder.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
                }
                builder.Append(">");
                if (Value == null) {
                    node.InnerText(ref builder);
                } else {
                    builder.Append(Value);
                }
                builder.AppendFormat("</{0}>", node.Name);
            }
        }

        public string OuterText() {
            var builder = new StringBuilder();
            InnerText(ref builder);
            return string.Format("<{0}>{1}</{0}>", Name, builder);
        }

    }

    /// <summary>
    ///     XML element attribute
    /// </summary>
    internal class NanoXmlAttribute {
        private readonly string name;
        private readonly string value;

        internal NanoXmlAttribute(string name, string value) {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        ///     Attribute name
        /// </summary>
        public string Name {
            get { return name; }
        }

        /// <summary>
        ///     Attribtue value
        /// </summary>
        public string Value {
            get { return value; }
        }
    }

    internal class XmlParsingException : Exception {
        public XmlParsingException(string message) : base(message) { }
    }
}