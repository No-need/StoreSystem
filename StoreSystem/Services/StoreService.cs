using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Database;
using StoreSystem.Models;
using StoreSystem.ViewModels;
using System.Drawing;

namespace StoreSystem.Services
{
    public class StoreService
    {
        private readonly StoreSystemContext _db;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, string> _orderStatus = new Dictionary<string, string>
        {
            {"","" },
            {"pending","待出貨" },
            {"shipped","已出貨" },
            {"completed","已完成" }
        };

        private readonly Dictionary<int, string> _clothSelect = new Dictionary<int, string>
        {
            {0,"排灣" },
            {1,"泰雅" },
            {2,"阿美" },
            {3,"魯凱" }
        };

        public StoreService(IConfiguration configuration,StoreSystemContext db) {  
            _db = db; 
            _config = configuration;
        }
        #region 查詢
        public List<ProductViewModel> GetProducts()
        {
            var productList =  (from p in _db.Products
                    select new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.ProductPrices.Select(x => x.Price).FirstOrDefault(),
                        Quantity = p.ProductStocks.Select(x=>x.Quantity).FirstOrDefault(),
                    }).ToList();
            foreach(var product in productList) { 
                var file = _db.ProductFiles.FirstOrDefault(x=>x.ProductId == product.ProductId);
                if (file != null)
                {
                    // 讀取檔案內容
                    byte[] fileBytes = File.ReadAllBytes(file.Path);

                    // 轉換為 Base64
                    string base64String = Convert.ToBase64String(fileBytes);

                    // 回傳完整的 Data URL
                    //product.ImageUrl = $"data:image/jpeg;base64,{base64String}";
                    product.ImageId = file.ProductFileId;
                }
            }
            return productList;
        }

        public ProductDetailPageViewModel GetDetailPage(long? id)
        {
            if(id == null)
            {
                return new ProductDetailPageViewModel() {
                    CategorySelect = GetCategorySelect(),
                    SizeSelect = GetSizeSelect(),
                };
            }

            var product = _db.Products
                .Include(x=>x.Product_Clothe)
                .Include(x=>x.ProductPrices)
                .Include(x=>x.ProductStocks)
                .FirstOrDefault(x=>x.ProductId == id);
            if(product == null)
            {
                throw new Exception("商品不存在");
            }
            return new ProductDetailPageViewModel
            {
                CategorySelect = GetCategorySelect(),
                SizeSelect = GetSizeSelect(),
                Detail = new ProductDetailViewModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Size = product.Product_Clothe.Size,
                    Category = product.Category,
                    Description = product.Description,
                    Price = product.ProductPrices.Select(x=>x.Price).FirstOrDefault()??0,
                    SKU = product.ProductPrices.Select(x=>x.Currency).FirstOrDefault(),
                    Quantity = product.ProductStocks.Select(x=>x.Quantity).FirstOrDefault()??0,
                }
            };
        }

        public List<OrderViewModel> GetOrders()
        {
            var orders = (from o in _db.Orders
                          select new OrderViewModel
                          {
                              OrderId = o.OrderId,
                              OrderMember = o.OrderMember,
                              OrderDate = o.OrderDate.GetValueOrDefault().ToString("yyyy/MM/dd"),
                              OrderStatus = o.OrderStatus,
                              Products = o.OrderProducts.Select(x=>new OrderProductViewModel
                              {
                                  ProductId = x.ProductId,
                                  Name = x.Product.Name,
                                  Price = x.Price,
                                  Quantity = x.Quantity
                              }).ToList()
                          }).ToList();
            return orders;
        }

        public OrderDetailPageViewModel GetOrderDetail(long orderId)
        {
            var order = _db.Orders.Include(x=>x.OrderProducts).ThenInclude(x=>x.Product).FirstOrDefault(x => x.OrderId == orderId);
            if (order == null) throw new Exception("訂單不存在");
            var model = new OrderDetailPageViewModel
            {
                OrderId = orderId,
                OrderMember = order.OrderMember,
                OrderDate = order.OrderDate.GetValueOrDefault().ToString("yyyy/MM/dd"),
                OrderStatus = _orderStatus[order.OrderStatus],
                PayMethod = order.PayMethod,
                PayStatus = order.PayStatus,
                ReceiveAddress = order.ReceiveAddress,
                ReceiveMember = order.ReceiveMember,
                ReceivePhone = order.ReceivePhone,
                ShippingMethod = order.ShippingMethod,
                ShippingStatus = order.ShippingStatus,
                OrderProducts = order.OrderProducts.Select(x=>new OrderProductDetailViewModel
                {
                    Name = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                }).ToList()
            };

            return model;
        }

        public List<SelectListItem> GetCategorySelect()
        {
            return new List<SelectListItem>
            {
                new SelectListItem ("請選擇", ""),
                new SelectListItem("upper_body","上衣"),
                new SelectListItem("lower_body","下身")
            };
        }

        public List<SelectListItem> GetSizeSelect()
        {
            return new List<SelectListItem>
            {
                new SelectListItem ("請選擇", ""),
                new SelectListItem("L","L"),
                new SelectListItem("M","M"),
                new SelectListItem("S","S"),
            };
        }
        #endregion

        #region 新增、修改、刪除

        public void CreateProduct(ProductCreateModel model)
        {
            var now = DateTime.Now;

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                CreateTime = now,
                Product_Clothe = new Product_Clothe
                {
                    Size = model.Size,
                    CreateTime = DateTime.Now,
                },
                ProductPrices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Currency = model.SKU,
                        Price = model.Price,
                        CreateTime = now,
                    }
                },
                ProductStocks = new List<ProductStock>
                {
                    new ProductStock
                    {
                        Quantity = model.Quantity,
                        CreateTime = now
                    }
                },
            };

            List<ProductFile> files = new List<ProductFile>();
            foreach (var f in model.Images)
            {
                var foldPath = Path.Combine(_config.GetValue<string>("UploadFilePath"), "Product", Guid.NewGuid().ToString());
                var path = Path.Combine(foldPath, f.FileName);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(foldPath);
                }
                using (var stream = System.IO.File.OpenWrite(path))
                {
                    f.CopyTo(stream);
                }
                files.Add(new ProductFile { FileName = f.FileName, CreateTime = now, Path = path, Product = product });
            }

            _db.Products.Add(product);
            _db.ProductFiles.AddRange(files);
            _db.SaveChanges();
        }

        public void UpdateProduct(ProductUpdateModel model) {
            var product = _db.Products.Include(x => x.Product_Clothe)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductStocks).FirstOrDefault(x => x.ProductId == model.ProductId);
            if (product == null)
            {
                throw new Exception("商品不存在");
            }
            var now = DateTime.Now;

            product.Name = model.Name;
            product.Description = model.Description;
            product.Category = model.Category;
            product.UpdateTime = now;

            var cloth = product.Product_Clothe;
            cloth.Size = model.Size;
            cloth.UpdateTime = now;

            var price = product.ProductPrices.FirstOrDefault();
            price.Currency = model.SKU;
            price.Price = model.Price;
            price.UpdateTime = now;

            var stock = product.ProductStocks.FirstOrDefault();
            stock.Quantity = model.Quantity;
            stock.UpdateTime = now;

            List<ProductFile> files = new List<ProductFile>();
            foreach (var f in model.Images)
            {
                var foldPath = Path.Combine(_config.GetValue<string>("UploadFilePath"), "Product", Guid.NewGuid().ToString());
                var path = Path.Combine(foldPath, f.FileName);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(foldPath);
                }
                using (var stream = System.IO.File.OpenWrite(path))
                {
                    f.CopyTo(stream);
                }
                files.Add(new ProductFile { FileName = f.FileName, CreateTime = now, Path = path, Product = product });
            }
            _db.ProductFiles.AddRange(files);
            _db.Products.Update(product);
            _db.SaveChanges();
        }

        public void DeleteProduct(long productId)
        {
            var product = _db.Products.Include(x => x.Product_Clothe)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductStocks)
                .Include(x=>x.ProductFiles).FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                throw new Exception("商品不存在");
            }
            _db.ProductFiles.RemoveRange(product.ProductFiles);
            _db.ProductPrices.RemoveRange(product.ProductPrices);
            _db.ProductStocks.RemoveRange(product.ProductStocks);
            _db.Product_Clothes.Remove(product.Product_Clothe);
            _db.Products.Remove(product);
            _db.SaveChanges();
        }

        #endregion

        #region 定單
        public void CreateOrder(OrderCreateModel model)
        {
            var order = new Order
            {
                OrderMember = model.OrderMember,
                OrderStatus = "pending",
                OrderDate = DateTime.Now.Date,
                PayMethod = model.PayMethod,
                PayStatus = "待付款",
                CreateTime = DateTime.Now,
                ReceiveAddress = model.ReceiveAddress,
                ReceiveMember = model.ReceiveMember,
                ReceivePhone = model.ReceivePhone,
                ShippingMethod = model.ShippingMethod,
                ShippingStatus = "未出貨",
                OrderProducts = model.OrderProducts.Select(x=>new OrderProduct
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = _db.ProductPrices.Where(y=>y.ProductId == x.ProductId).Select(x=>x.Price).FirstOrDefault()
                }).ToList()
            };
            _db.Orders.Add(order);
            _db.SaveChanges();
        }
        #endregion

        #region 對外api

        public ClothDataViewModel GetClothData(ClothSearchCondModel cond)
        {
            var products = _db.Products.Include(x => x.Product_Clothe).Include(x=>x.ProductFiles).Where(x => cond.Index == null || x.Name.Contains(_clothSelect[cond.Index.GetValueOrDefault()])).ToList();
            var group = products.GroupBy(x => x.Name);
            var model = new ClothDataViewModel
            {
                Images = group.Select(x=>new ClothImageViewModel
                {
                    Name = x.Key,
                    Category = x.Select(y=>y.Category).FirstOrDefault(),
                    ImageUrl =  x.SelectMany(y=>y.ProductFiles.Select(z=>z.Path)).FirstOrDefault(),
                    Size = x.ToDictionary(x=>x.Product_Clothe.Size,x=>x.ProductId)
                }).ToList()
            };

            foreach(var image in model.Images)
            {
                // 讀取檔案內容
                byte[] fileBytes = File.ReadAllBytes(image.ImageUrl);

                // 轉換為 Base64
                string base64String = Convert.ToBase64String(fileBytes);

                // 回傳完整的 Data URL
                image.ImageUrl = $"data:image/jpeg;base64,{base64String}";

            }
            return model;
        }
        #endregion
    }
}
