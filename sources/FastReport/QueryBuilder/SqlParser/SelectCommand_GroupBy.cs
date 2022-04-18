﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SelectCommand_GroupBy : ParserBase
    {

        public override bool CanParse(SqlParser parser)
        {
            return parser.Token.Type == SqlTokenType.Keyword && parser.Token.Text == "group by";
        }

        public override void Parse(SqlParser parser)
        {
            SqlToken token = parser.NextToken();

            while(token.Type != SqlTokenType.EOF)
            {
                if (token.Type == SqlTokenType.Keyword)
                    break;

                SqlParser.FieldStruct field = ReadField(parser, ref token);

                parser.Groups.Add(field);

                if (token.Type == SqlTokenType.Punctuation && token.Text == ",")
                    token = parser.NextToken();
            }
        }

        public SqlParser.FieldStruct ReadField(SqlParser parser, ref SqlToken token)
        {
            string table = null;
            string variable = null;

            
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

            SqlParser.FieldStruct result = new SqlParser.FieldStruct();
            result.Table = table;
            result.Name = variable;
            return result;
        }
    }
}