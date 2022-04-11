using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class Phrase
    {
        /// <summary>
        /// Ставка за 1000 показов (CPM) или 1000 кликов (CPC). Единица измерения — одна миллионная доля рубля, округляется до копеек. Обязательное поле.
        /// </summary>
        public string bid;
        /// <summary>
        /// Поисковая фраза. Обязательное поле.
        /// </summary>
        public string phrase;
        public string relevanceStatus;

        public Phrase(string bid, string phrase)
        {
            this.bid = bid ?? throw new ArgumentNullException(nameof(bid));
            this.phrase = phrase ?? throw new ArgumentNullException(nameof(phrase));
            relevanceStatus = string.Empty;
        }
    }
}