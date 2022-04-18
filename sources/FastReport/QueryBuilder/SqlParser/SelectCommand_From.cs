using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SelectCommand_From : ParserBase
    {

        
        public override bool CanParse(SqlParser parser)
        {
            return parser.Token.Type == SqlTokenType.Keyword && parser.Token.Text == "from";
        }
        public override void Parse(SqlParser parser)
        {
            SqlToken token = parser.NextToken();
            
            while (token.Type != SqlTokenType.EOF)
            {
                if (token.Type == SqlTokenType.Keyword)
                    return;

                ReadGroup(parser, ref token);

                if (token.Type == SqlTokenType.Punctuation && token.Text == ",")
                    token = parser.NextToken();
            }

        }

        /// <summary>
        /// read current token, returns next
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public void ReadGroup(SqlParser parser, ref SqlToken token)
        {
            while (token.Type == SqlTokenType.Punctuation && token.Text == ",")
                token = parser.NextToken();

            if (token.Type == SqlTokenType.Punctuation && token.Text == "(")
            {
                token = parser.NextToken();
                //read group
                ReadGroup(parser, ref token);
                if (token.Type == SqlTokenType.Punctuation && token.Text == ")")
                    token = parser.NextToken();
            }
            else
            {
                ReadTable(parser, ref token);
            }

            if(token.Type == SqlTokenType.Keyword)
            {
                string table = null;
                QueryEnums.JoinTypes joinType = QueryEnums.JoinTypes.Where;
                switch (token.LowerText)
                {
                    case "inner join":
                        token = parser.NextToken();
                        table = ReadTable(parser, ref token);
                        joinType = QueryEnums.JoinTypes.InnerJoin;
                        break;
                    case "left outer join":
                        token = parser.NextToken();
                        table = ReadTable(parser, ref token);
                        joinType = QueryEnums.JoinTypes.LeftOuterJoin;
                        break;
                    case "right outer join":
                        token = parser.NextToken();
                        table = ReadTable(parser, ref token);
                        joinType = QueryEnums.JoinTypes.RightOuterJoin;
                        break;
                    case "full outer join":
                        token = parser.NextToken();
                        table = ReadTable(parser, ref token);
                        joinType = QueryEnums.JoinTypes.FullOuterJoin;
                        break;
                }

                if(table != null )
                {
                    if(token.Type != SqlTokenType.Keyword || token.LowerText != "on" )
                    {
                        parser.ThrowFormat(token, "on");
                    }

                    SqlParser.FieldStruct one = ReadField(parser, ref token);

                    QueryEnums.WhereTypes op;

                    token = parser.NextToken();
                    switch (token.LowerText)
                    {
                        //,       "<>",          ">=",                ">",         "<=",             "<",      "LIKE", "NOT LIKE"
                        case "=": op = QueryEnums.WhereTypes.Equal; break;
                        case "<>": op = QueryEnums.WhereTypes.NotEqual; break;
                        case ">=": op = QueryEnums.WhereTypes.GreaterOrEqual; break;
                        case ">": op = QueryEnums.WhereTypes.Greater; break;
                        case "<=": op = QueryEnums.WhereTypes.LessOrEqual; break;
                        case "<": op = QueryEnums.WhereTypes.Less; break;
                        case "like": op = QueryEnums.WhereTypes.Like; break;
                        case "not like": op = QueryEnums.WhereTypes.NotLike; break;
                        default: parser.ThrowFormat(token, "where types"); return;
                    }

                    SqlParser.FieldStruct two = ReadField(parser, ref token);

                    //read token after field , or ) or next command
                    token = parser.NextToken();

                    SqlParser.LinkStruct link = new SqlParser.LinkStruct();
                    link.One = one;
                    link.Two = two;
                    link.WhereType = op;
                    link.JoinType = joinType;
                    link.Table = table;

                    parser.Links.Add(link);
                }
            }
        }

        /// <summary>
        /// read current token, returns next
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ReadTable(SqlParser parser, ref SqlToken token)
        {
            string table = null;
            string alias = null;

            
            if (token.Type != SqlTokenType.Name)
                parser.ThrowFormat(token, "name");
            table = token.Text;

            token = parser.NextToken();
            if (token.Type == SqlTokenType.Name)
            {
                alias = token.Text;
                token = parser.NextToken();
            }

            SqlParser.TableStruct tableStruct = new SqlParser.TableStruct();
            tableStruct.Name = table;
            tableStruct.Alias = alias;

            parser.Tables.Add(tableStruct);
            return table;
        }

        /// <summary>
        /// ignore current token, read from next, returns current i.e not next
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public SqlParser.FieldStruct ReadField(SqlParser parser, ref SqlToken token)
        {
            string table = null;
            string variable = null;

            token = parser.NextToken();
            if (token.Type != SqlTokenType.Name)
                parser.ThrowFormat(token, "name");

            table = token.Text;

            token = parser.NextToken();
            if (token.Type != SqlTokenType.Punctuation || token.Text != ".")
                parser.ThrowFormat(token, ".");


            token = parser.NextToken();
            if (token.Type != SqlTokenType.Name)
                parser.ThrowFormat(token, "name");

            variable = token.Text;

            SqlParser.FieldStruct result = new SqlParser.FieldStruct();
            result.Table = table;
            result.Name = variable;
            return result;
        }

    }
}
