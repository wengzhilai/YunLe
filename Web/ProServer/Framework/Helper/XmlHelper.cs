using System;
using System.Xml;

namespace ProServer.Helper
{
    /// <summary>
    /// XML助手
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 将布尔转成字符串 true:"true" false:"false"
        /// </summary>
        /// <param name="b">布尔值</param>
        /// <returns>转化后的字符串</returns>
        public static string BoolToString(bool b)
        {
            return b ? "true" : "false";
        }

        /// <summary>
        /// 将字符串转成布尔 "true","1":true "false","0":false
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>转化后的布尔值</returns>
        public static bool StringToBool(string s)
        {
            return s == "1" || string.Compare(s, "true", true) == 0;
        }

        /// <summary>
        /// XML文档助手
        /// </summary>
        public static class Document
        {
            /// <summary>
            /// 创建XmlDocument
            /// </summary>
            /// <returns>返回XmlDocument</returns>
            public static XmlDocument Create()
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateProcessingInstruction("xml", "version='1.0' encoding='utf-8'"));
                return doc;
            }

            /// <summary>
            /// 创建XmlDocument及root层节点
            /// </summary>
            /// <param name="rootName">root节点名称,为空时默认为"root"</param>
            /// <param name="root">创建的root节点</param>
            /// <returns>返回XmlDocument</returns>
            public static XmlDocument CreateAndRoot(string rootName, out XmlNode root)
            {
                XmlDocument doc = Create();
                if (string.IsNullOrEmpty(rootName))
                    rootName = "root";
                root = Node.Add(doc, null, rootName);
                return doc;
            }

            /// <summary>
            /// 保存为Xml形式的字符串
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <returns>字符串</returns>
            public static string Save(XmlDocument doc)
            {
                return doc.InnerXml;
            }

            /// <summary>
            /// 添加Xml形式的字符串,并返回创建的XmlDocument
            /// </summary>
            /// <param name="xmlString">Xml形式的字符串</param>
            /// <returns>XmlDocument</returns>
            public static XmlDocument Load(string xmlString)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                return doc;
            }

            /// <summary>
            /// 添加Xml形式的字符串,并返回创建的XmlDocument及root节点
            /// </summary>
            /// <param name="xmlString">Xml形式的字符串</param>
            /// <param name="root">root节点</param>
            /// <returns>XmlDocument</returns>
            public static XmlDocument Load(string xmlString, out XmlNode root)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                root = doc.DocumentElement;
                return doc;
            }

            /// <summary>
            /// 添加Xml形式的字符串,并返回创建的XmlDocument及root节点
            /// </summary>
            /// <param name="xmlString">Xml形式的字符串</param>
            /// <param name="root">root节点</param>
            /// <returns>XmlDocument</returns>
            public static XmlDocument LoadFile(string xmlPath, out XmlNode root)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                root = doc.DocumentElement;
                return doc;
            }
        }

        /// <summary>
        /// XML节点助手
        /// </summary>
        public static class Node
        {
            #region add node

            /// <summary>
            /// 为parent添加子节点,如果parent为空就创建root层节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name)
            {
                return parent == null ? doc.AppendChild(doc.CreateElement("root")) : parent.AppendChild(doc.CreateElement(name));
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, string value)
            {
                XmlNode rr = parent.AppendChild(doc.CreateElement(name));
                if (!string.IsNullOrEmpty(value))
                    rr.InnerText = value;
                return rr;
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, bool value)
            {
                return Add(doc, parent, name, BoolToString(value));
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, double value)
            {
                return Add(doc, parent, name, value.ToString());
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, float value)
            {
                return Add(doc, parent, name, value.ToString());
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, int value)
            {
                return Add(doc, parent, name, value.ToString());
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, DateTime value)
            {
                return Add(doc, parent, name, value, "yyyy-MM-dd HH:mm:ss");
            }

            /// <summary>
            /// 为parent添加子节点
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="name">节点名</param>
            /// <param name="value">节点值</param>
            /// <param name="format">格式</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string name, DateTime value, string format)
            {
                if (string.IsNullOrEmpty(format))
                    format = "yyyy-MM-dd HH:mm:ss";
                return Add(doc, parent, name, value.ToString(format));
            }

            #endregion

            #region add node and attr

            /// <summary>
            /// 为parent添加子节点(无节点值),并为新节点添加属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="attrName">属性名</param>
            /// <param name="attrValue">属性值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string nodeName, string attrName, string attrValue)
            {
                XmlNode rr = Add(doc, parent, nodeName);
                Attr.Add(doc, rr, attrName, attrValue);
                return rr;
            }

            /// <summary>
            /// 为parent添加子节点(无节点值),并为新节点添加属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="attrName">属性名</param>
            /// <param name="attrValue">属性值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string nodeName, string attrName, bool attrValue)
            {
                return Add(doc, parent, nodeName, attrName, BoolToString(attrValue));
            }

            /// <summary>
            /// 为parent添加子节点(无节点值),并为新节点添加属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="parent">父</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="attrName">属性名</param>
            /// <param name="attrValue">属性值</param>
            /// <returns>节点</returns>
            public static XmlNode Add(XmlDocument doc, XmlNode parent, string nodeName, string attrName, int attrValue)
            {
                return Add(doc, parent, nodeName, attrName, attrValue.ToString());
            }

            #endregion

            #region get node

            /// <summary>
            /// 获得parent下面指定名称的节点
            /// </summary>
            /// <param name="parent">父节点</param>
            /// <param name="name">节点名称</param>
            public static XmlNode GetNode(XmlNode parent, string name)
            {
                for (var i = parent.ChildNodes.Count - 1; i >= 0; i--)
                    if (parent.ChildNodes[i].Name == name)
                        return parent.ChildNodes[i];
                return null;
            }

            /// <summary>
            /// 获得parent下面指定名称指定属性值的节点
            /// </summary>
            /// <param name="parent">父节点</param>
            /// <param name="name">节点名称</param>
            /// <param name="attrName">属性名</param>
            /// <param name="attrValue">属性值</param>
            /// <returns></returns>
            public static XmlNode GetNode(XmlNode parent, string name, string attrName, string attrValue)
            {
                for (var i = parent.ChildNodes.Count - 1; i >= 0; i--)
                {
                    XmlNode node = parent.ChildNodes[i];
                    if (node.NodeType == XmlNodeType.Element && node.Name.SameText(name) && Attr.GetValue(node, attrName, string.Empty).SameText(attrValue))
                        return parent.ChildNodes[i];
                }
                return null;
            }

            /// <summary>
            /// 取parent下指定nodeName的值(字符串)
            /// </summary>
            /// <param name="parent">父节点</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static string GetNodeValue(XmlNode parent, string nodeName, string def)
            {
                XmlNode node = GetNode(parent, nodeName);
                return node == null ? def : GetValue(node, def);
            }

            /// <summary>
            /// 取parent下指定nodeName的值(布尔)
            /// </summary>
            /// <param name="parent">父节点</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static bool GetNodeValue(XmlNode parent, string nodeName, bool def)
            {
                XmlNode node = GetNode(parent, nodeName);
                return node == null ? def : GetValue(node, def);
            }

            /// <summary>
            /// 取parent下指定nodeName的值(整数)
            /// </summary>
            /// <param name="parent">父节点</param>
            /// <param name="nodeName">节点名</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static int GetNodeValue(XmlNode parent, string nodeName, int def)
            {
                XmlNode node = GetNode(parent, nodeName);
                return node == null ? def : GetValue(node, def);
            }

            #endregion

            #region get value

            /// <summary>
            /// 取节点的值(字符串)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static string GetValue(XmlNode node, string def)
            {
                return string.IsNullOrEmpty(node.InnerText) ? def : node.InnerText;
            }

            /// <summary>
            /// 取节点的值(布尔)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static bool GetValue(XmlNode node, bool def)
            {
                return string.IsNullOrEmpty(node.InnerText) ? def : StringToBool(node.InnerText);
            }

            /// <summary>
            /// 取节点的值(整数)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="def">节点默认值</param>
            /// <returns>节点值</returns>
            public static int GetValue(XmlNode node, int def)
            {
                return string.IsNullOrEmpty(node.InnerText) ? def : Convert.ToInt32(node.InnerText);
            }

            #endregion
        }

        /// <summary>
        /// XML节点属性助手
        /// </summary>
        public static class Attr
        {
            #region add

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, string value)
            {
                XmlAttribute attr = node.Attributes.Append(doc.CreateAttribute(name));
                attr.Value = value;
                return attr;
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, bool value)
            {
                return Add(doc, node, name, BoolToString(value));
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, double value)
            {
                return Add(doc, node, name, value.ToString());
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, float value)
            {
                return Add(doc, node, name, value.ToString());
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, int value)
            {
                return Add(doc, node, name, value.ToString());
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, DateTime value)
            {
                return Add(doc, node, name, value, "yyyy-MM-dd HH:mm:ss");
            }

            /// <summary>
            /// 为节点添加一个属性
            /// </summary>
            /// <param name="doc">XmlDocument</param>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="value">属性值</param>
            /// <param name="format">格式</param>
            /// <returns>属性</returns>
            public static XmlAttribute Add(XmlDocument doc, XmlNode node, string name, DateTime value, string format)
            {
                if (string.IsNullOrEmpty(format))
                    format = "yyyy-MM-dd HH:mm:ss";
                return Add(doc, node, name, value.ToString(format));
            }

            #endregion

            #region get value

            /// <summary>
            /// 取节点的属性值(字符串)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="def">属性默认值</param>
            /// <returns>属性值</returns>
            public static string GetValue(XmlNode node, string name, string def)
            {
                XmlAttribute att = node.Attributes[name];
                return att != null ? att.Value : def;
            }

            /// <summary>
            /// 取节点的属性值(整数)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="def">属性默认值</param>
            /// <returns>属性值</returns>
            public static int GetValue(XmlNode node, string name, int def)
            {
                XmlAttribute att = node.Attributes[name];
                return att != null ? Convert.ToInt32(att.Value) : def;
            }

            /// <summary>
            /// 取节点的属性值(布尔)
            /// </summary>
            /// <param name="node">节点</param>
            /// <param name="name">属性名</param>
            /// <param name="def">属性默认值</param>
            /// <returns>属性值</returns>
            public static bool GetValue(XmlNode node, string name, bool def)
            {
                XmlAttribute att = node.Attributes[name];
                return att != null ? StringToBool(att.Value) : def;
            }

            #endregion
        }
    }
}