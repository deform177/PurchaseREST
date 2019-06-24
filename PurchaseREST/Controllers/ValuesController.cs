using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseREST.Model;

namespace PurchaseREST.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        PurchasesContext pc;

        public ValuesController(PurchasesContext _pc)
        {
            pc = _pc;
        }

        [Route("fill")]
        [HttpGet]
        public void Fill()
        {
            for (int i = 0; i < 10000; i++)
            {
                var purchase = new Purchases { Name = "компьютер", Date = DateTime.Now, Details = "intel i9, 32 GB RAM", Price = 44555.55m, UserId=1 };
                pc.Add(purchase);
            }
            pc.SaveChanges();

        }

        [Route("getpurchase")]
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetPurchase()
        {
            var purchases = await Task.Run(() => GetAsyncPurchase());
            var result = purchases.ToList().Select(item => new
            {
                purchase = item.PurchaseId,
                name = item.Name,
                date = item.Date.ToString("yyyy-MM-dd HH:mm"),
                details = item.Details,
                price = item.Price.ToString()
            }).ToArray();

            return Json(result);
        }

        private IEnumerable<Purchases> GetAsyncPurchase()
        {
            var userId = Int32.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var purchases = pc.Purchases.AsNoTracking().Where(q => q.UserId == userId).ToList();
            return purchases;
        }

        [Route("addpurchase")]
        [HttpPost]
        [Authorize]
        public JsonResult AddPurchase(Purchase purchase)
        {
            var priceDecimal = Decimal.Parse(purchase.price.Replace(',', '.'), CultureInfo.InvariantCulture);

            if (ModelState.IsValid)
            {
                if (purchase.purchaseid == 0)
                {
                    var userId = Int32.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);

                    Purchases newPurchase = new Purchases
                    {
                        Name = purchase.name,
                        Date = purchase.date,
                        Details = purchase.details,
                        Price = priceDecimal,
                        UserId = userId

                    };
                    pc.Add(newPurchase);
                    pc.SaveChanges();

                    return Json(new { purchaseid = newPurchase.PurchaseId });
                }
                else
                {
                    Purchases editPurchase = pc.Purchases.FirstOrDefault(q => q.PurchaseId == purchase.purchaseid);
                    editPurchase.Name = purchase.name;
                    editPurchase.Date = purchase.date;
                    editPurchase.Details = purchase.details;
                    editPurchase.Price = priceDecimal;
                    pc.SaveChanges();

                    return Json(new { purchaseid = purchase.purchaseid });
                }
            }
            return Json(new { purchaseid = 0 });

        }

        [Route("deletepurchase")]
        [HttpPost]
        [Authorize]
        public JsonResult DeletePurchase(int purchaseid)
        {
            var purchase = pc.Purchases.FirstOrDefault(q => q.PurchaseId == purchaseid); 
            pc.Purchases.Remove(purchase);
            pc.SaveChanges();
            return Json(new { message = "ok" });
        }
        }
}
