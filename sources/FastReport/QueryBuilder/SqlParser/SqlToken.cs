using System.Collections.Generic;

namespace FastReport.FastQueryBuilder
{
    internal enum SqlTokenType
    {
        Keyword,
        Punctuation,
        Name,
        Operation,
        EOF
    }

    internal class SqlToken
    {
        #region Private Fields

        private string lowerText;
        private int position;
        private string text;
        private SqlTokenType type;

        #endregion Private Fields

        #region Public Properties

        public string LowerText
        {
            get
            {
                if (lowerText == null)
                    lowerText = text.ToLower();
                return lowerText;
            }
        }

        public int Position
        {
            get { return position; }
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public SqlTokenType Type
        {
            get { return type; }
        }

        #endregion Public Properties

        #region Public Constructors

        public SqlToken(SqlTokenType type, string text, int position)
        {
            this.type = type;
            this.text = text;
            this.position = position;
        }

        public override bool Equals(object obj)
        {
            SqlToken token = obj as SqlToken;
            return token != null &&
                   LowerText == token.LowerText &&
                   Type == token.Type;
        }

        public override int GetHashCode()
        {
            int hashCode = -969693818;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LowerText);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        #endregion Public Constructors



    }
}