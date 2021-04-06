using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;

namespace EpubConverter.Model
{
    public class XmlModel
    {
        public string InitVersion { get; set; }
        public IList<XmlElement> Elements { get; set; }

        public XmlModel()
        {
            this.Elements = new List<XmlElement>();
        }

        public bool IsElementExist(string elementName)
        {
            if (this.Elements.Any(x => x.Name == elementName))
                return true;
            else
                foreach (XmlElement ele in this.Elements)
                    if (ele.Childrens != null && ele.Childrens.Any(x => x.Name == elementName))
                        return true;
            return false;
        }

        //public XmlElement FindChild(XmlElement xmlElement, string elementName)
        //{
        //    if (xmlElement.Childrens != null)
        //    {
        //        return FindChild(xmlElement.Childrens, elementName);
        //    }
        //    if(xmlElement.Name == )
        //}

        public string GetValue(string elementName)
        {
            var targetElement = GetElement(elementName);
            if (targetElement == null)
                return null;

            return targetElement.Value;
        }

        public string GetAttributeValue(string elementName, string attributeName)
        {
            XmlElement targetElement = GetElement(elementName);

            if (targetElement == null)
                return null;

            if (targetElement.Attributes.Any(x => x.Name == attributeName))
                return targetElement.Attributes.Single(x => x.Name == attributeName).Value;
            else
                return null;
        }

        public XmlElement GetElement(string elementName)
        {//TODO: 應該要查出List 全部Element
            if (this.Elements.Any(x => x.Name == elementName))
            {
                return this.Elements.Single(x => x.Name == elementName);
            }
            else
            {
                foreach (var ele in this.Elements)
                {
                    if (ele.Childrens.Any(x => x.Name == elementName))
                    {
                        return ele.Childrens.Single(x => x.Name == elementName);
                    }
                }
            }

            return null;
        }
    }

    public class XmlElement
    {
        public IList<XmlElement> Childrens { get; set; }

        public IList<XmlAttribute> Attributes { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public XmlElement()
        {
            this.Name = string.Empty;
            this.Attributes = new List<XmlAttribute>();

            //TODO: 不知道會不會 無限迴圈
            this.Childrens = new List<XmlElement>();
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(this.Name) && this.Attributes.Count <= 0 && this.Childrens.Count <= 0;
        }

        public XmlElement GetElement(string elementName)
        {
            if (this.Name == elementName)
            {
                return this;
            }
            else if (this.Childrens.Any(x => x.Name == elementName))
            {
                return this.Childrens.Single(x => x.Name == elementName);
            }
            else
            {
                foreach (XmlElement ele in this.Childrens)
                {
                    if (ele.Childrens.Any(x => x.Name == elementName))
                    {
                        return ele.Childrens.Single(x => x.Name == elementName);
                    }
                }
            }

            return null;
        }
    }

    public class XmlAttribute
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
