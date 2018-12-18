using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class JsObject : Is4jToken
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

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public List<Is4jToken> Items { get; set; }

        public S4JState State { get; set; }

        public JsObject()
        {
            Text = "";
            Items = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            this.Items.Add(Child);
        }

        public void AppendCharToToken(Char Char)
        {
            //this.Text += Char;
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
            foreach (var child in Items)
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
