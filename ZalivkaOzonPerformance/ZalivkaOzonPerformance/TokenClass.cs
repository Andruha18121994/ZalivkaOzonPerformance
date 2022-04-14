using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class TokenClass
    {
        public string access_token;
        public int expires_in;
        public string token_type;

        //пустой конструктор необходим для десериализации json параметров переданных с ответа Озон АПИ
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public TokenClass()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        {
        }

        public TokenClass(TokenClass tokenClass)
        {
            this.access_token = tokenClass.access_token;
            this.expires_in = tokenClass.expires_in;
            this.token_type = tokenClass.token_type;
        }

        public TokenClass(string access_token, int expires_in, string token_type)
        {
            this.access_token = access_token ?? throw new ArgumentNullException(nameof(access_token));
            this.expires_in = expires_in;
            this.token_type = token_type ?? throw new ArgumentNullException(nameof(token_type));
        }
    }
}
