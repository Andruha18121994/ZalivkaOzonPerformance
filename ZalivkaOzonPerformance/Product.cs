using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class Product
    {
        /// <summary>
        /// SKU рекламируемого товара.
        /// </summary>
        public string sku;
        /// <summary>
        /// Ставка за 1000 показов (CPM) или 1000 кликов (CPC). Единица измерения — одна миллионная доля рубля, округляется до копеек.
        /// </summary>
        public string bid;
        /// <summary>
        /// Идентификатор ранее созданной группы товаров: набор стоп-слов и поисковых фраз со ставками, общий для нескольких товаров.
        /// </summary>
        public string groupId;
        /// <summary>
        /// Список поисковых фраз со ставками.
        /// </summary>
        public List<Phrase> phrases;
        /// <summary>
        /// Список стоп-слов.
        /// </summary>
        public List<string> stopWords;


        public Product(string sku, string bid, List<Phrase> phrases)
        {
            this.sku = sku ?? throw new ArgumentNullException(nameof(sku));
            this.bid = bid ?? throw new ArgumentNullException(nameof(bid));
            this.groupId = string.Empty;
            this.phrases = phrases ?? throw new ArgumentNullException(nameof(phrases));
            this.stopWords = new List<string>();
        }
    }
}
