using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    public class CampaignForCreate
    {
        /// <summary>
        /// Название рекламной кампании.
        /// </summary>
        public string title;
        /// <summary>
        /// Ограничение дневного бюджета рекламной кампании. Единица измерения — одна миллионная доля рубля, округляется до копеек. Если поле не задано, дневной бюджет не ограничен.
        /// </summary>
        public string dailyBudget;
        /// <summary>
        /// Способ распределения бюджета: DAILY_BUDGET — бюджет равномерно распределяется по дням; ASAP — быстрая открутка, бюджет не ограничен по дням. По умолчанию — DAILY_BUDGET.
        /// </summary>
        public string expenseStrategy;
        /// <summary>
        /// Место размещения рекламируемых товаров: PLACEMENT_PDP — карточка товара; PLACEMENT_SEARCH_AND_CATEGORY — поиск и категории.
        /// </summary>
        public string placement;
        /// <summary>
        /// !!! только для построения внутренней модели!
        /// </summary>
        public List<Product> products;
        /// <summary>
        /// !!! только для построения внутренней модели!
        /// </summary>
        public string campaignId;


        public CampaignForCreate(string title, string dailyBudget, string expenseStrategy, string placement)
        {
            this.title = title ?? throw new ArgumentNullException(nameof(title));
            this.dailyBudget = dailyBudget ?? throw new ArgumentNullException(nameof(dailyBudget));
            this.expenseStrategy = expenseStrategy ?? throw new ArgumentNullException(nameof(expenseStrategy));
            this.placement = placement ?? throw new ArgumentNullException(nameof(placement));
            this.products = new List<Product>();
            this.campaignId = string.Empty;
        }

        public void AddId(string campaignId)
        {
            this.campaignId = campaignId;
        }
        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException();
            products.Add(product);
        }
    }
}
