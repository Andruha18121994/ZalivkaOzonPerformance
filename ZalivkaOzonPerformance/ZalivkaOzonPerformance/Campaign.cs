using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class Campaign
    {
        /// <summary>
        /// Название рекламной кампании.
        /// </summary>
        string title;
        /// <summary>
        /// Ограничение дневного бюджета рекламной кампании. Единица измерения — одна миллионная доля рубля, округляется до копеек. Если поле не задано, дневной бюджет не ограничен.
        /// </summary>
        string dailyBudget;
        /// <summary>
        /// Способ распределения бюджета: DAILY_BUDGET — бюджет равномерно распределяется по дням; ASAP — быстрая открутка, бюджет не ограничен по дням. По умолчанию — DAILY_BUDGET.
        /// </summary>
        string expenseStrategy;
        /// <summary>
        /// Место размещения рекламируемых товаров: PLACEMENT_PDP — карточка товара; PLACEMENT_SEARCH_AND_CATEGORY — поиск и категории.
        /// </summary>
        string placement;
        /// <summary>
        /// !!! только для построения внутренней модели!
        /// </summary>
        List<Product> products;
        /// <summary>
        /// !!! только для построения внутренней модели!
        /// </summary>
        string campaignId;


    }
}
