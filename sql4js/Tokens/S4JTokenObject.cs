using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenObject : Is4jToken
    {
        /*public object this[string index]
        {
            get
            {
                var key = this.Items.FirstOrDefault(i => index.Equals2(i.Key));
                return key?.Value;
            }
            set
            {
                var key = this.Items.FirstOrDefault(i => index.Equals2(i.Key));
                if (key == null)
                {
                    key = new IJsToken();
                    this.Items.Add(key);
                }
                key.Key = index;
                key.Value = value;
            }

        }*/
        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }
        
        public S4JState State { get; set; }

        public S4JTokenObject()
        {
            Text = "";
            Children = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            this.Children.Add(Child);
        }


        public void AppendCharsToToken(IList<Char> Chars)
        {

        }

        public void CommitToken()
        {
            IsCommited = true;
        }

        public void BuildJson(StringBuilder Builder)
        {
            Builder.Append("{");
            Int32 i = 0;
            Boolean prevWasKey = true;
            foreach (var child in Children)
            {
                if (!prevWasKey) Builder.Append(",");
                child.BuildJson(Builder);
                if (child.IsKey) { prevWasKey = true; Builder.Append(":"); }
                else { prevWasKey = false; }
                i++;
            }
            Builder.Append("}");
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }

    /*public class JsObjectKeyValue
    {
        public Object Key { get; set; }

        public Object Value { get; set; }

        public Boolean OnlyKey { get; set; }
    }*/
}
