using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//http://stackoverflow.com/questions/13704752/deserialize-xml-to-object-using-dynamic
//https://blogs.msdn.microsoft.com/mcsuksoldev/2010/02/04/dynamic-xml-reader-with-c-and-net-4-0/
namespace Pollux
{
    public class DynamicXml : DynamicObject
    {
        XElement _root;

        //private IDictionary<string, object> _objectMembers = new Dictionary<string, object>();

        private DynamicXml(XElement root)
        {
            _root = root;

            //if (root.HasAttributes)
            //{
            //    foreach (XAttribute attr in root.Attributes())
            //    {
            //        _objectMembers.Add(attr.Name.LocalName, (object)attr.Value);
            //    }
            //}
        }
        public static DynamicXml Parse(string xmlString)
        {
            return new DynamicXml(XDocument.Parse(xmlString).Root);
        }

        public static DynamicXml Load(string filename)
        {
            return new DynamicXml(XDocument.Load(filename).Root);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var att = _root.Attribute(binder.Name);

            if (att != null)
            {
                result = (dynamic)att.Value;
                return true;
            }

            var nodes = _root.Elements(binder.Name);
            if (nodes.Count() > 0)
            {
                //result = nodes.Select(n => n.HasElements ? new DynamicXml(n) : n.Value).ToList();
                result = nodes.Select(n => new DynamicXml(n)).ToList();
                return true;
            }
            else
            {
                result = null;
                //return false;
            }
            /*
                    var node = _root.Element(binder.Name);
                    if (node != null)
                    {
                        //result = node.HasElements ? (dynamic)new DynamicXml(node) : node.Value;
                        result = (dynamic)new DynamicXml(node);
                        return true;
                    }
                    */
            return true;
        }

        public override string ToString()
        {
            if (_root != null)
            {
                return _root.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public string this[string attr]
        {
            get
            {
                if (_root == null)
                {
                    return string.Empty;
                }
                if (_root.Attribute(attr) == null)
                {
                    return string.Empty;
                }

                return _root.Attribute(attr).Value;
            }

        }
    }
    public class XmlToDynamic
    {
        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }
    }
}
