using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class CampaignFromOzon
    {
        /// <summary>
        /// Идентификатор кампании
        /// </summary>
        public string id;
        /// <summary>
        /// Название рекламной кампании.
        /// </summary>
        public string title;
        /// <summary>
        /// Состояние кампании:
        ///CAMPAIGN_STATE_RUNNING — активная кампания;
        ///CAMPAIGN_STATE_PLANNED — кампания, сроки проведения которой ещё не наступили;
        ///CAMPAIGN_STATE_STOPPED — кампания, сроки проведения которой завершились;
        ///CAMPAIGN_STATE_INACTIVE — кампания, остановленная владельцем;
        ///CAMPAIGN_STATE_ARCHIVED — архивная кампания;
        ///CAMPAIGN_STATE_MODERATION_DRAFT — отредактированная кампания до отправки на модерацию;
        ///CAMPAIGN_STATE_MODERATION_IN_PROGRESS — кампания, отправленная на модерацию;
        ///CAMPAIGN_STATE_MODERATION_FAILED — кампания, непрошедшая модерацию;
        ///CAMPAIGN_STATE_FINISHED — кампания завершена, дата окончания в прошлом, такую кампанию нельзя изменить, можно только клонировать или создать новую.
        /// </summary>
        public string state;
        /// <summary>
        ///Тип рекламируемной кампании, фактически — тип рекламируемого объекта:
        ///SKU — реклама товаров в спонсорских полках c размещением на карточках товаров, в поиске или категории;
        ///BANNER — баннерная рекламная кампания;
        ///SIS — реклама магазина;
        ///BRAND_SHELF — брендовая полка;
        ///BOOSTING_SKU — повышение товаров в каталоге;
        ///ACTION — рекламная кампания для акций продавца;
        ///ACTION_CAS — рекламная кампания для акции.
        /// </summary>
        public string advObjectType;
        /// <summary>
        /// Дата старта рекламной кампании.
        /// </summary>
        public string fromDate;
        /// <summary>
        /// Дата завершения рекламной кампании
        /// </summary>
        public string toDate;
        /// <summary>
        /// Бюджет рекламной кампании. Единица измерения — одна миллионная доля рубля, округляется до копеек.
        /// </summary>
        public string budget;
        /// <summary>
        /// Дневной бюджет рекламной кампании. Единица измерения — одна миллионная доля рубля, округляется до копеек.
        /// </summary>
        public string dailyBudget;
        /// <summary>
        /// Место размещения рекламы:
        ///
        ///PLACEMENT_INVALID — не определено;
        ///PLACEMENT_PDP — карточка товара;
        ///PLACEMENT_SEARCH_AND_CATEGORY — поиск и категории
        /// </summary>
        public string placement;
        /// <summary>
        /// Дата создания кампании в формате RFC3339.
        /// </summary>
        public string createdAt;
        /// <summary>
        /// Дата обновления кампании в формате RFC3339.
        /// </summary>
        public string updatedAt;
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public CampaignFromOzon()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        {
        }
    }
}
