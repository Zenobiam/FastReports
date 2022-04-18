using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SqlParser
    {
        public string qch;
        private string sql;
        private List<FieldStruct> fields;
        private List<TableStruct> tables;
        private List<LinkStruct> links;
        private List<FieldStruct> groups;
        private List<FieldStruct> orders;
        private List<FieldStruct> where;
        int tokenPosition = 0;

        public  SqlParser(string sql)
        {
            this.sql = sql;
        }

        public string SQL
        {
            get { return sql; }
        }

        private List<SqlToken> tokens;

        public SqlToken Token
        {
            get
            {
                if (tokenPosition < tokens.Count)
                    return tokens[tokenPosition];
                return new SqlToken(SqlTokenType.EOF, "", sql.Length);
            }
        }

        public List<FieldStruct> Fields { get
            {
                return fields;
            }
        }
        public List<LinkStruct> Links
        {
            get
            {
                return links;
            }
        }

        public List<FieldStruct> Groups
        {
            get
            {
                return groups;
            }
        }

        public List<FieldStruct> Orders
        {
            get
            {
                return orders;
            }
        }

        public List<TableStruct> Tables { get { return tables; } }
        public List<FieldStruct> Where { get { return where; } }

        public SqlToken NextToken()
        {
            tokenPosition++;
            return Token;
        }

        public int Position
        {
            get { return tokenPosition; }
            set { tokenPosition = value; }
        }

        

        public Query Parse()
        {
            fields = new List<FieldStruct>();
            tables = new List<TableStruct>();
            links = new List<LinkStruct>();
            groups = new List<FieldStruct>();
            orders = new List<FieldStruct>();
            where = new List<FieldStruct>();

            SqlLexer lexer = new SqlLexer(sql, qch);
            tokens = lexer.Parse();
            tokenPosition = 0;
            SelectCommand selectParser = new SelectCommand();
            if (selectParser.CanParse(this))
                selectParser.Parse(this);
            else ThrowFormat(Token, "select");

            return null;
        }

   


       



        

        public void ThrowFormat(SqlToken token, string expectToken)
        {
            throw new FormatException("Incorect syntax at position " + token.Position.ToString() + ", unexpected token " + token .Text+ ", " + "expecting " + expectToken + "."  + Environment.NewLine + sql);
        }


       

        

        public class FieldStruct
        {
            private string name = string.Empty;
            private string alias = string.Empty;
            private string func = string.Empty;
            private string table = string.Empty;
            private string filter = string.Empty;

            private SortTypes sortType;

            public string Name { get { return name; } set { if (value == null) value = string.Empty; name = value; } }
            public string Alias { get { return alias; } set { if (value == null) value = string.Empty; alias = value; } }
            public string Func { get { return func; } set { if (value == null) value = string.Empty; func = value; } }
            public string Table { get { return table; } set { if (value == null) value = string.Empty; table = value; } }

            public SortTypes SortType { get { return sortType; } set { sortType = value; } }
            public string Filter { get { return filter; } set { filter = value; } }

            public override string ToString()
            {

                return Table + "." + Name;
            }
        }

        public class TableStruct
        {
            private string name = string.Empty;
            private string alias = string.Empty;


            public string Name { get { return name; } set { if (value == null) value = string.Empty; name = value; } }
            public string Alias { get { return alias; } set { if (value == null) value = string.Empty; alias = value; } }
        }

        public class LinkStruct
        {
            private QueryEnums.WhereTypes whereType;
            private QueryEnums.JoinTypes joinType;
            private FieldStruct one;
            private FieldStruct second;
            private string table = string.Empty;

            public QueryEnums.WhereTypes WhereType { get { return whereType; } set { whereType = value; } }
            public QueryEnums.JoinTypes JoinType { get { return joinType; } set { joinType = value; } }
            public FieldStruct One { get { return one; } set { one = value; } }
            public FieldStruct Two { get { return second; } set { second = value; } }
            public string Table { get { return table; } set { if (value == null) value = string.Empty; table = value; } }
        }
    }

    

    
}
