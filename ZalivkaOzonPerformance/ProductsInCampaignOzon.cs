using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    internal class ProductsInCampaignOzon
    {
        public List<Product> bids { get; set; }
        public string campaignId { get; set; }
        public ProductsInCampaignOzon(CampaignForCreate campaignForCreate)
        {
            this.campaignId = campaignForCreate.campaignId;
            this.bids = campaignForCreate.products;
        }
    }
}
