using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SqlLexer
    {
        string sql;
        int position;
        string qch;

        public SqlLexer(string sql, string qch)
        {
            this.sql = sql;
            this.qch = qch;
        }

        public List<SqlToken> Parse()
        {
            List<SqlToken> tokens = new List<SqlToken>();
            position = 0;

            SkipWhiteSpace();

            SqlToken token;
            while (position < sql.Length)
            {

                token = IsOperation();
                if (token == null)
                    token = ReadCommand();
                if (token == null)
                    token = ReadName();
                if (token == null)
                    token = GetPunctuation();
                if (token == null)
                    throw new FormatException("Incorect syntax at position " + position.ToString() + ", unexpected character " +sql[position].ToString()  + Environment.NewLine + sql);

                SkipWhiteSpace();
                tokens.Add(token);
            }

            return tokens;
        }

        private SqlToken GetPunctuation()
        {
            if (position < sql.Length && IsPunctuation(sql[position]))
            {
                position++;
                return new SqlToken(SqlTokenType.Punctuation, sql[position - 1].ToString(), position - 1);
            }
            return null;
        }

        private SqlToken IsOperation()
        {

            for (int i = 0; i < operations.Length; i++)
            {
                if (position + operations[i].Length <= sql.Length)
                {
                    for (int j = 0; j < operations[i].Length; j++)
                    {
                        if (Char.ToLower(sql[position + j]) != operations[i][j])
                            goto nextOperation;
                    }

                    if (position + operations[i].Length >= sql.Length ||
                        Char.IsWhiteSpace(sql[position + operations[i].Length])
                        || IsPunctuation(sql[position + operations[i].Length])
                        || (IsPunctuation(sql[position + operations[i].Length - 1]) && !IsPunctuation(sql[position + operations[i].Length])))
                    {
                        SqlToken result = new SqlToken(SqlTokenType.Operation, sql.Substring(position, operations[i].Length), position);
                        position += operations[i].Length;
                        return result;
                    }
                }


               

            nextOperation:
                {

                }
            }
            return null;
        }

        private string[] operations = new string[] { "||", "*", "/", "+", "-", "<>", ">", "<", ">=", "<=", "==", "=", "!=", "in", "nin", "is", "like", "nlike", "not", "or", "and" };
        private string[] keywords = new string[] { "and", "as", "asc", "between", "case", "collate nocase", "desc", "else", "end", "from", "group by", "having", "in", "not in", "is", "limit", "offset", "like", "not between", "not like", "on", "or", "order by", "select", "then", "union", "union all", "using", "when", "where", "with", "join", "full join", "cross join", "inner join", "left join", "right join", "full outer join", "right outer join", "left outer join", };

        private bool IsPunctuation(char c)
        {
            return " ~`!@#$%^&*()-+=[]{};:'\",.<>/?\\|".IndexOf(c) != -1;
        }

        public SqlToken ReadName()
        {
            StringBuilder result = new StringBuilder();
            int pos = position;

            if (position < sql.Length && sql[position] == qch[0])
            {
                position++;
                // read to quote
                // TODO not ignore double quote
                while (position < sql.Length)
                {
                    if (sql[position] == qch[1])
                    {
                        position++;
                        break;
                    }
                    result.Append(sql[position]);
                    position++;
                }
                
            }
            else
            {
                while (position < sql.Length)
                {
                    if (Char.IsWhiteSpace(sql[position]) || IsPunctuation(sql[position]))
                    {
                        break;
                    }
                    result.Append(sql[position]);
                    position++;
                }
            }
            if (result.Length > 0)
                return new SqlToken(SqlTokenType.Name, result.ToString(), pos);
            return null;
        }

        private SqlToken ReadCommand()
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if (position + keywords[i].Length  <= sql.Length)
                {
                    for (int j = 0; j < keywords[i].Length ; j++)
                    {
                        if (Char.ToLower(sql[position + j]) != keywords[i][j])
                            goto nextKeyword;
                    }

                    if (position + keywords[i].Length   >= sql.Length ||
                        Char.IsWhiteSpace(sql[position + keywords[i].Length])
                        || IsPunctuation(sql[position + keywords[i].Length]))
                    {
                        SqlToken result = new SqlToken(SqlTokenType.Keyword, keywords[i], position);
                        position += keywords[i].Length;
                        return result;
                    }
                }
              
            nextKeyword:
                {

                }
            }
            return null;
        }

        private void SkipWhiteSpace()
        {
            while (position < sql.Length && char.IsWhiteSpace(sql[position]))
            {
                position++;
            }
        }
    }
}
