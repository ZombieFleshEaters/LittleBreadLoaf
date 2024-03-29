using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutModel : PageModel
    {
        private const string EftPosDisplay = "EFTPOS - on delivery / pickup - credit card accepted";

        private const string DeliveryTimePreOrder = "Scheduled";

        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public CartCheckoutModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public littlebreadloaf.Data.Cart Cart { get; set; }
        
        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> PaymentMethodOptions { get; set; }

        [BindProperty]
        public string Full_Address { get; set; }

        [BindProperty]
        public BusinessSettings BusinessSettings { get; set; }

        [BindProperty]
        public bool IsPreOrder { get; set; }

        [BindProperty]
        public string PreOrderSource { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;

            var source = HttpContext.Request.Cookies[CartHelper.PreOrderCookie];
            var cartID = HttpContext.Request.Cookies[CartHelper.CartCookieName];

            if (string.IsNullOrEmpty(cartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            Guid.TryParse(cartID, out Guid parsedCartID);
            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(f => f.CartID == parsedCartID);

            if(ProductOrder != null && ProductOrder.ContactAddress > 0)
            {
                var nzAddress = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);
                if(nzAddress != null)
                {
                    Full_Address = nzAddress.full_address;
                }
            }

            if(ProductOrder != null)
            {
                if (ProductOrder.DeliveryDate == new DateTime(9999, 12, 31))
                    ProductOrder.DeliveryDate = null;
                if (ProductOrder.PickupDate == new DateTime(9999, 12, 31))
                    ProductOrder.PickupDate = null;
            }
            else
            {
                ProductOrder = new ProductOrder();
                //ProductOrder.DeliveryTime = DeliveryTime;
                if (IsPreOrder)
                {
                    ProductOrder.DeliveryTime = DeliveryTimePreOrder;
                    var preOrderSource = await _context.PreOrderSource.AsNoTracking().Where(w => w.Source == source).FirstOrDefaultAsync();
                    if (preOrderSource != null)
                    {
                        PreOrderSource = preOrderSource.Source;
                        ProductOrder.DeliveryInstructions = $"Pre order: {source}";
                        ProductOrder.ContactAddress = preOrderSource.AddressID;

                        var address = await _context.NzAddressDeliverable.Where(w => w.address_id == preOrderSource.AddressID).FirstOrDefaultAsync();
                        if (address != null)
                            Full_Address = address.full_address;
                    }
                }
            }

            PaymentMethodOptions = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = false },
                new SelectListItem(){ Text = "VOUCHER", Value = "Voucher", Selected = false },
                new SelectListItem(){ Text = "EFTPOS", Value = EftPosDisplay, Selected = false },
            },"Text","Value",null);

            if (IsPreOrder)
            {
                PaymentMethodOptions = new SelectList(new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = true }
                }, "Text", "Value", "BANK");
            }

            BusinessSettings = await _context.BusinessSettings.AsNoTracking().FirstOrDefaultAsync();


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;

            BusinessSettings = await _context.BusinessSettings.AsNoTracking().FirstOrDefaultAsync();

            //ProductOrder.DeliveryTime = DeliveryTime;

            var validDeliveryDaysOfWeek = new List<DayOfWeek>();
            if (BusinessSettings.DeliverSunday) validDeliveryDaysOfWeek.Add(DayOfWeek.Sunday);
            if (BusinessSettings.DeliverMonday) validDeliveryDaysOfWeek.Add(DayOfWeek.Monday);
            if (BusinessSettings.DeliverTuesday) validDeliveryDaysOfWeek.Add(DayOfWeek.Tuesday);
            if (BusinessSettings.DeliverWednesday) validDeliveryDaysOfWeek.Add(DayOfWeek.Wednesday);
            if (BusinessSettings.DeliverThursday) validDeliveryDaysOfWeek.Add(DayOfWeek.Thursday);
            if (BusinessSettings.DeliverFriday) validDeliveryDaysOfWeek.Add(DayOfWeek.Friday);
            if (BusinessSettings.DeliverSaturday) validDeliveryDaysOfWeek.Add(DayOfWeek.Saturday);

            if(IsPreOrder)
            {
                var source = HttpContext.Request.Cookies[CartHelper.PreOrderCookie];
                var preOrder = await _context.PreOrderSource.Where(w => w.Source == source).FirstOrDefaultAsync();
                if(preOrder != null)
                {
                    validDeliveryDaysOfWeek = new List<DayOfWeek>();
                    if (preOrder.Sunday) validDeliveryDaysOfWeek.Add(DayOfWeek.Sunday);
                    if (preOrder.Monday) validDeliveryDaysOfWeek.Add(DayOfWeek.Monday);
                    if (preOrder.Tuesday) validDeliveryDaysOfWeek.Add(DayOfWeek.Tuesday);
                    if (preOrder.Wednesday) validDeliveryDaysOfWeek.Add(DayOfWeek.Wednesday);
                    if (preOrder.Thursday) validDeliveryDaysOfWeek.Add(DayOfWeek.Thursday);
                    if (preOrder.Friday) validDeliveryDaysOfWeek.Add(DayOfWeek.Friday);
                    if (preOrder.Saturday) validDeliveryDaysOfWeek.Add(DayOfWeek.Saturday);
                }

                ProductOrder.DeliveryTime = DeliveryTimePreOrder;
                var preOrderSource = await _context.PreOrderSource.AsNoTracking().Where(w => w.Source == source).FirstOrDefaultAsync();
                if (preOrderSource != null)
                {
                    PreOrderSource = preOrderSource.Source;
                    ProductOrder.DeliveryInstructions = $"Pre order: {source}";
                    ProductOrder.ContactAddress = preOrderSource.AddressID;

                    var address = await _context.NzAddressDeliverable.Where(w => w.address_id == preOrderSource.AddressID).FirstOrDefaultAsync();
                    if (address != null)
                        Full_Address = address.full_address;
                }

            }

            var validPickupDaysOfWeek = new List<DayOfWeek>();
            if (BusinessSettings.PickupSunday) validPickupDaysOfWeek.Add(DayOfWeek.Sunday);
            if (BusinessSettings.PickupMonday) validPickupDaysOfWeek.Add(DayOfWeek.Monday);
            if (BusinessSettings.PickupTuesday) validPickupDaysOfWeek.Add(DayOfWeek.Tuesday);
            if (BusinessSettings.PickupWednesday) validPickupDaysOfWeek.Add(DayOfWeek.Wednesday);
            if (BusinessSettings.PickupThursday) validPickupDaysOfWeek.Add(DayOfWeek.Thursday);
            if (BusinessSettings.PickupFriday) validPickupDaysOfWeek.Add(DayOfWeek.Friday);
            if (BusinessSettings.PickupSaturday) validPickupDaysOfWeek.Add(DayOfWeek.Saturday);

            PaymentMethodOptions = new SelectList(new List<SelectListItem>()
            {
                //new SelectListItem(){ Text = "CASH", Value = "Cash - on delivery / pickup", Selected = false },
                new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = false },
                new SelectListItem(){ Text = "VOUCHER", Value = "Voucher", Selected = false },
                new SelectListItem(){ Text = "EFTPOS", Value = EftPosDisplay, Selected = false },
            }, "Text", "Value", null);

            if(IsPreOrder)
            {
                PaymentMethodOptions = new SelectList(new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = true }
                }, "Text", "Value", "BANK");
                var test = TryValidateModel(ProductOrder);
            }

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);

                return Page();
            }

            var cartID = HttpContext.Request.Cookies[CartHelper.CartCookieName];
            if (string.IsNullOrEmpty(cartID) || !Guid.TryParse(cartID, out Guid parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            //DATE
            if (!ProductOrder.PickupDate.HasValue && !ProductOrder.DeliveryDate.HasValue)
            {
                ModelState.AddModelError("Validation.DeliveryOrPickup", "Choose either a delivery date or pickup date.");
                return Page();
            }

            var validPickupDate = (ProductOrder.PickupDate.HasValue && ProductOrder.PickupDate.Value < new DateTime(9999, 12, 31));
            var validDeliveryDate = (ProductOrder.DeliveryDate.HasValue && ProductOrder.DeliveryDate.Value < new DateTime(9999, 12, 31));

            if(validDeliveryDate && validPickupDate)
            {
                ModelState.AddModelError("Validation.DeliveryOrPickup", "Choose either a delivery date or pickup date.");
                return Page();
            }

            if (validPickupDate && ProductOrder.PickupDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("Validation.PickupDateInPast", "Pickup date cannot be in the past.");
                return Page();
            }

            if (validDeliveryDate && ProductOrder.DeliveryDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("Validation.DeliveryDateInPast", "Delivery date cannot be in the past.");
                return Page();
            }

            //TIME
            if (validPickupDate && string.IsNullOrEmpty(ProductOrder.PickupTime))
            {
                ModelState.AddModelError("Validation.PickupTimeRequired", "A pickup time is required.");
                return Page();
            }

            if(validDeliveryDate && string.IsNullOrEmpty(ProductOrder.DeliveryTime))
            {
                if (!IsPreOrder)
                {
                    ModelState.AddModelError("Validation.DeliveryTimeRequired", "A delivery time is required.");
                    return Page();
                }
                else
                {
                    if (IsPreOrder)
                        ProductOrder.DeliveryTime = DeliveryTimePreOrder;
                }
            }

            if(validPickupDate) 
            {
                ProductOrder.ContactAddress = 0; // If pickup, don't worry about a delivery address

                var dayOfWeek = ProductOrder.PickupDate.Value.DayOfWeek;
                if (!validPickupDaysOfWeek.Contains(dayOfWeek))
                {
                    if(IsPreOrder)
                    {
                        ModelState.AddModelError("Validation.PickupDayOfWeek", "Pickup date must be a weekday.");
                    }
                    else
                    {
                        ModelState.AddModelError("Validation.PickupDayOfWeek", "Invalid pickup date. The day chosen is not eligible for pickup.");
                    }
                    return Page();
                }
            }

            if(validDeliveryDate) //Delivery must have an address
            {
                if(ProductOrder.ContactAddress == null || ProductOrder.ContactAddress == 0)
                {
                    ModelState.AddModelError("Address.Missing", "Begin typing your address to validate.");
                    return Page();
                }

                var dayOfWeek = ProductOrder.DeliveryDate.Value.DayOfWeek;
                if (!validDeliveryDaysOfWeek.Contains(dayOfWeek))
                {
                    ModelState.AddModelError("Validation.DeliveryDayOfWeek", "Invalid delivery date. The day chosen is not eligible for delivery.");
                    return Page();
                }

                if(!IsPreOrder)
                {
                    var cartItems = await _context
                                            .CartItem
                                            .Where(w => w.CartID == parsedCartID)
                                            .Select(s => new { s.Price, s.Quantity })
                                            .ToListAsync();

                    if (cartItems.Sum(s => s.Price * s.Quantity) < BusinessSettings.MinimumDeliveryOrderAmount)
                    {
                        ModelState.AddModelError("Validation.DeliveryMinNotMet", $"The minimum delivery amount is ${BusinessSettings.MinimumDeliveryOrderAmount}");
                        return Page();
                    }
                }
            }

            if(!await _context.Cart.AnyAsync(a => a.CartID == parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            if (ProductOrder.DeliveryInstructions == null)
                ProductOrder.DeliveryInstructions = "";

            //Determine if pickup or delivery
            if (ProductOrder.DeliveryDate.HasValue)
            {
                ProductOrder.PickupDate = new DateTime(9999, 12, 31);
                ProductOrder.PickupTime = "";
            }else if(ProductOrder.PickupDate.HasValue)
            {
                ProductOrder.DeliveryDate = new DateTime(9999, 12, 31);
                ProductOrder.DeliveryTime = "";
            }

            if (await _context.ProductOrder.AnyAsync(f => f.CartID == parsedCartID))
            {
                ProductOrder.CartID = parsedCartID;
                ProductOrder.ContactEmail = ProductOrder.ContactEmail.ToLower();
                ProductOrder.Payment = new DateTime(9999, 12, 31);
                ProductOrder.Confirmed = new DateTime(9999, 12, 31);
                ProductOrder.ConfirmationCode = ""; //Default to empty, this will be set when checked out

                _context.ProductOrder.Update(ProductOrder);
            }
            else
            {
                ProductOrder.ContactEmail = ProductOrder.ContactEmail.ToLower();
                ProductOrder.OrderID = Guid.NewGuid();
                ProductOrder.CartID = parsedCartID;
                ProductOrder.Payment = new DateTime(9999, 12, 31);
                ProductOrder.Confirmed = new DateTime(9999, 12, 31);
                ProductOrder.Created = DateTime.Now;
                ProductOrder.ConfirmationCode = ""; //Default to empty, this will be set when checked out

                _context.ProductOrder.Add(ProductOrder);
            }
            
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Cart/CartCheckoutReview", new { ProductOrderID = ProductOrder.OrderID, CartID = ProductOrder.CartID });
        }
    }
}