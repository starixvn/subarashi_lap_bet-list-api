using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Resources;
using Fanex.Common;

namespace Fanex.Athena.Common.DataAccess
{
    public class ProductAccess : BaseDataAccess, IProductAccess
    {
        public IList<Product> GetProductAccessList(int licenseeId)
        {
            // All licensees have product Sb
            var allowedProducts = new List<Product> { new Product { IsOfferedSingleWallet = true, ProductCode = "SB", ProductName = ViewResources.Sportsbook } };

            using (var db = this.GetObjectDb("Lrf_LicenseeProduct3rdSel"))
            {
                db.SetParameterValue("licenseeid", licenseeId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allowedProducts.Add(new Product
                        {
                            ProductID = Converter.ToInt(reader["ProductID"]),
                            ProductName = DataHelper.GetProductName(Converter.ToString2(reader["productprefix"])),
                            ProductCode = Converter.ToString2(reader["productprefix"]),
                            AllowCommission = Converter.ToBool(reader["comm"]),
                            AllowWinloss = Converter.ToBool(reader["winloss"]),
                            IsOfferedSingleWallet = Converter.ToBool(reader["IsOfferedSingleWallet"])
                        });
                    }
                }

                return allowedProducts;
            }
        }
    }
}
