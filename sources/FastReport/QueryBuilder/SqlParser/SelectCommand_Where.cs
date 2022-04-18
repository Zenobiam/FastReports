using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SelectCommand_Where : ParserBase
    {

        public override bool CanParse(SqlParser parser)
        {
            
            return parser.Token.Type == SqlTokenType.Keyword && parser.Token.LowerText == "where";
        }

        public override void Parse(SqlParser parser)
        {
            SqlToken token = parser.NextToken();

            while (token.Type != SqlTokenType.EOF)
            {
                if (token.Type == SqlTokenType.Keyword)
                    break;

                SqlParser.FieldStruct field = ReadField(parser, ref token);

                if (field == null)
                    return;

                QueryEnums.WhereTypes op;
                

                if (token.Type != SqlTokenType.Operation)
                    parser.ThrowFormat(token, "operation");
                string opText = token.Text;
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

                token = parser.NextToken();

                SqlParser.FieldStruct second = ReadField(parser, ref token);
                if(second != null)
                {
                    SqlParser.LinkStruct link = new SqlParser.LinkStruct();
                    link.JoinType = QueryEnums.JoinTypes.Where;
                    link.WhereType = op;
                    link.One = field;
                    link.Two = second;
                    parser.Links.Add(link);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(opText).Append(" ");
                    int level = 0;
                    SqlToken last_token;
                    while (token.Type!= SqlTokenType.EOF)
                    {
                        sb.Append(token.Text);
                        last_token = token;
                        if (token.Type == SqlTokenType.Punctuation)
                        {
                            if (token.Text == "(") level++;
                            else if (token.Text == ")") level--;
                        }
                        token = parser.NextToken();

                        int whiteSpaces = token.Position - last_token.Position - last_token.Text.Length;
                        sb.Append(' ', whiteSpaces);

                        if (level == 0 && token.Type == SqlTokenType.Operation && token.LowerText == "and")
                        {
                            int lastPos = parser.Position;

                            token = parser.NextToken();
                            if (ReadField(parser, ref token) != null)
                            {
                                parser.Position = lastPos;
                                token = parser.Token;
                                break;
                            }
                        }
                        else if(level == 0 && token.Type == SqlTokenType.Keyword)
                        {
                            break;
                        }
                    }

                    field.Filter = sb.ToString();

                    parser.Where.Add(field);

                    // add sb to field
                }

                if (token.Type != SqlTokenType.Operation || token.LowerText != "and")
                    return;
                token = parser.NextToken();
            }
        }

        public SqlParser.FieldStruct ReadField(SqlParser parser, ref SqlToken token)
        {
            int pos = parser.Position;
            string table = null;
            string variable = null;


            if (token.Type != SqlTokenType.Name)
            {
                parser.Position = pos;
                token = parser.Token;
                return null;
                //parser.ThrowFormat(token, "name");
            }

            table = token.Text;

            token = parser.NextToken();
            if (token.Type != SqlTokenType.Punctuation || token.Text != ".")
            {
                parser.Position = pos;
                token = parser.Token;
                return null;
                //parser.ThrowFormat(token, ".");
            }


            token = parser.NextToken();
            if (token.Type != SqlTokenType.Name)
            {
                parser.Position = pos;
                token = parser.Token;
                return null;
                //parser.ThrowFormat(token, "name");
            }

            variable = token.Text;

            token = parser.NextToken();

            SqlParser.FieldStruct result = new SqlParser.FieldStruct();
            result.Table = table;
            result.Name = variable;
            return result;
        }
    }
}
