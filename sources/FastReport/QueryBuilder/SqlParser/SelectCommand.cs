using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SelectCommand : ParserBase
    {

        public override bool CanParse(SqlParser parser)
        {
            return parser.Token.LowerText == "select";
        }

        public override void Parse(SqlParser parser)
        {
            SqlToken token = parser.NextToken();

            while (token.Type != SqlTokenType.EOF)
            {
                if (token.Type == SqlTokenType.Keyword)
                {
                    if (token.LowerText == "from")
                    {
                        break;
                    }
                    else
                    {
                        parser.ThrowFormat(token, "from");
                    }
                }
                // parse expression
                string function = null;
                string table = null;
                string variable = null;
                string alias = null;

                if (token.Type != SqlTokenType.Name)
                {
                    parser.ThrowFormat(token, "name");
                }

                function = token.Text;

                token = parser.NextToken();

                if (token.Type == SqlTokenType.Punctuation && token.Text == "(")
                {
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

                    token = parser.NextToken();
                    if (token.Type != SqlTokenType.Punctuation || token.Text != ")")
                        parser.ThrowFormat(token, ")");
                }
                else if (token.Type == SqlTokenType.Punctuation && token.Text == ".")
                {

                    table = function;
                    function = null;

                    token = parser.NextToken();
                    if (token.Type != SqlTokenType.Name)
                        parser.ThrowFormat(token, "name");

                    variable = token.Text;
                }
                else
                {
                    parser.ThrowFormat(token, ". or (");
                }

                token = parser.NextToken();

                if (token.Type == SqlTokenType.Keyword && token.Text == "as")
                {

                    token = parser.NextToken();
                    if (token.Type != SqlTokenType.Name)
                        parser.ThrowFormat(token, "name");

                    alias = token.Text;
                    token = parser.NextToken();
                }

                SqlParser.FieldStruct field = new SqlParser.FieldStruct();
                field.Alias = alias;
                field.Name = variable;
                field.Func = function;
                field.Table = table;

                parser.Fields.Add(field);

                if (token.Type == SqlTokenType.Punctuation && token.Text == ",")
                {
                    token = parser.NextToken();
                }
            }



            if (parser.Token.Type != SqlTokenType.EOF)
            {
                SelectCommand_From selectCommand_From = new SelectCommand_From();
                if (selectCommand_From.CanParse(parser))
                {
                    selectCommand_From.Parse(parser);
                }
                else
                {
                    parser.ThrowFormat(parser.Token, "from");
                }


                SelectCommand_GroupBy groupBy = new SelectCommand_GroupBy();
                SelectCommand_OrderBy orderBy = new SelectCommand_OrderBy();
                SelectCommand_Where where = new SelectCommand_Where();

                while (parser.Token.Type != SqlTokenType.EOF)
                {
                    if (groupBy.CanParse(parser))
                        groupBy.Parse(parser);
                    else if (orderBy.CanParse(parser))
                        orderBy.Parse(parser);
                    else if (where.CanParse(parser))
                        where.Parse(parser);
                    else parser.ThrowFormat(token, "group by, order by or where");
                }
            }
        }
    }
}
