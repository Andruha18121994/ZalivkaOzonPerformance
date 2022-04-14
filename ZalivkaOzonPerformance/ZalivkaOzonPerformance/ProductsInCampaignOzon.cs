using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZalivkaOzonPerformance
{
    public class ProductsInCampaignOzon
    {
        public List<Product> bids { get; set; }
        public string campaignId { get; set; }

        public ProductsInCampaignOzon(CampaignForCreate campaignForCreate)
        {
            this.campaignId = campaignForCreate.campaignId;
            this.bids = new List<Product>();

            foreach (Product product in campaignForCreate.products)
            {
                List<Phrase> phrases = new List<Phrase>();
                foreach (Phrase phrase in product.phrases)
                {
                    if (phrase != null)
                    {
                        phrases.Add(new Phrase(phrase.bid, phrase.phrase));
                    }
                }
                this.bids.Add(new Product(product.sku, product.bid, phrases));
            }
        }
    }
}
