﻿using System;
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
        /// <summary>
        ///string
        ///Статус релевантности — соответствия поисковой фразы рекламируемому товару:
        ///relevant — релевантно;
        ///not_relevant — не релевантно;
        ///in_progress — релевантность ещё не определена;
        ///on_moderation — релевантность ещё не определена, необходима ручная модерация.
        /// </summary>
        public string relevanceStatus;
    }
}
