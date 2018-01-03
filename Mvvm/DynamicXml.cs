using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//http://stackoverflow.com/questions/13704752/deserialize-xml-to-object-using-dynamic
//https://blogs.msdn.microsoft.com/mcsuksoldev/2010/02/04/dynamic-xml-reader-with-c-and-net-4-0/
//https://blogs.msdn.microsoft.com/csharpfaq/2009/10/19/dynamic-in-c-4-0-creating-wrappers-with-dynamicobject/
namespace Pollux
{
    //XElement to DynamicObject Wrapper 
    //[DebuggerDisplay("{debuggerDisplayProxy}")]
    public class DynamicXml : DynamicObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly object LockObject;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        XElement _root;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public XElement Node { get { return _root; } }

        //private KeyCollection debuggerDisplayProxy 
        //{ 
        //    get 
        //    {
        //        return new KeyCollection(this); 
        //    } 
        //}

        //private IDictionary<string, object> _objectMembers = new Dictionary<string, object>();

        private DynamicXml(XElement root)
        {
            LockObject = new object();

            _root = root;
        }
        public static DynamicXml Parse(string xmlString)
        {
            return new DynamicXml(XDocument.Parse(xmlString).Root);
        }

        public static DynamicXml Load(string filename)
        {
            return new DynamicXml(XDocument.Load(filename).Root);
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var att = _root.Attribute(binder.Name);

            if (att != null)
            {

                result = (string)att.Value;
                return true;
            }

            var nodes = _root.Elements(binder.Name);

            if (nodes.Count() > 0)
            {
                //result = nodes.Select(n => n.HasElements ? new DynamicXml(n) : n.Value).ToList();
                //多個XElements
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
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //https://blogs.msdn.microsoft.com/csharpfaq/2009/10/19/dynamic-in-c-4-0-creating-wrappers-with-dynamicobject/            
            //未完成
            XElement setNode = this._root.Element(binder.Name);

            if (setNode != null)
                setNode.SetValue(value);
            else
            {
                if (value.GetType() == typeof(DynamicXml))
                    this._root.Add(new XElement(binder.Name));
                else
                    this._root.Add(new XElement(binder.Name, value));
            }
            return true;
        }

        //convert DynamicXml to ????
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            //result = Convert.ChangeType(_target, binder.Type)
            if (binder.Type == typeof(String))
            {
                result = this._root.Value;
                return true;
            }
            else if (binder.Type == typeof(XElement))
            {
                result = this._root;
                
                return true;
            }
            else
            {
                result = null;
                return false;

            }

        }
        public override bool TryInvokeMember(InvokeMemberBinder binder,object[] args,out object result)
        {
            Type xmlType = typeof(XElement);

            try
            {
                result = xmlType.InvokeMember(binder.Name,BindingFlags.InvokeMethod |BindingFlags.Public |BindingFlags.Instance,null, this._root, args);
                return true;
            }

            catch
            {
                result = null;
                return false;
            }

        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var elements = this._root.Elements().Select(i => i.Name.LocalName);
            var attributes = this._root.Attributes().Select(i => i.Name.LocalName);

            return elements.Union(attributes);
        }

/*
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
*/
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



        [DebuggerTypeProxy("KeyCollectionDebugView")]
        [DebuggerDisplay("Count = {Count}")]
        public class KeyCollection : ICollection<string>
        {
            Dictionary<string, object> Attributes { get; set; }

            private readonly DynamicXml dynamicXmlNode;
            private readonly XElement _expandoData;

            internal KeyCollection(DynamicXml dynamicXml)
            {
                Attributes = new Dictionary<string, object>();
                lock (dynamicXml.LockObject)
                {
                    dynamicXmlNode = dynamicXml;
                    _expandoData = dynamicXml.Node;
                    
                    var root = dynamicXml.Node;

                    if (root.HasAttributes)
                    {
                        foreach (XAttribute attr in root.Attributes())
                        {
                            Attributes.Add(attr.Name.LocalName, attr.Value);
                        }
                    }
                }
            }

            #region ICollection<string> Members

            public void Add(string item)
            {
            }

            public void Clear()
            {
            }

            public bool Contains(string item)
            {
                lock (dynamicXmlNode.LockObject)
                {
                    return (dynamicXmlNode.Node).Attributes(item).Count()>0;
                }
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
            }

            public int Count
            {
                get
                {
                    return (dynamicXmlNode.Node).Attributes().Count();
                }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(string item)
            {
                return false;
            }

            #endregion

            #region IEnumerable<string> Members

            public IEnumerator<string> GetEnumerator()
            {
                var root = this.dynamicXmlNode.Node;

                if (root.HasAttributes)
                {
                    foreach (XAttribute attr in root.Attributes())
                    {
                        yield return attr.Name.LocalName;
                    }
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }
    }
        
    public sealed class KeyCollectionDebugView
    {
        private ICollection<string> collection;
        public KeyCollectionDebugView(ICollection<string> collection)
        {
            Debug.Assert(collection != null);
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public string[] Items
        {
            get
            {
                string[] items = new string[collection.Count];
                collection.CopyTo(items, 0);
                return items;
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
