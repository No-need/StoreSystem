using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Database;
using StoreSystem.Helper;
using StoreSystem.Models;
using StoreSystem.Services;

namespace StoreSystem.Controllers
{
    public class StoreController : Controller
    {
        private readonly StoreService _service;
        private readonly StoreSystemContext _db;
        public StoreController(StoreSystemContext db,StoreService storeService) { 
            _db = db;
            _service = storeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult storefront()
        {

            return View();
        }

        public IActionResult cart()
        {
            return View();
        }

        public IActionResult checkout()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult login()
        {
            return PartialView();
        }

        [AllowAnonymous]
        public IActionResult ValidPassword([FromBody]LoginModel model)
        {
            var user = _db.SYS_Users.FirstOrDefault(x=>x.UserId == model.UserId);

            if (user == null) throw new Exception("帳號不存在");

            var hash = HashHelper.ToMD5(model.Password);
            if (user.Password != hash) throw new Exception("密碼錯誤");
            HttpContext.Session.SetString("UserId", model.UserId);
            return Ok();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect("login");
        }

        public IActionResult merchant_dashboard()
        {
            return View();
        }

        public IActionResult order_success()
        {
            return View();
        }

        public IActionResult product_form([FromQuery]long? ProductId)
        {
            var model = _service.GetDetailPage(ProductId);
            return View(model);
        }

        public IActionResult orderlist()
        {
            return View();
        }

        public IActionResult order(long id)
        {
            var result = _service.GetOrderDetail(id);
            return View(result);
        }

        [HttpPost]
        public IActionResult GetProducts()
        {
            var result = _service.GetProducts();
            return Json(result);
        }

        public IActionResult GetOrders()
        {
            var result = _service.GetOrders();
            return Json(result);
        }

        public IActionResult GetOrderDetail(long id)
        {
            var result = _service.GetOrderDetail(id);
            return Json(result);
        }

        #region 商品CRUD

        public IActionResult CreateProduct([FromForm]ProductCreateModel model)
        {
            _service.CreateProduct(model);
            return Ok();
        }

        public IActionResult UpdateProduct([FromForm]ProductUpdateModel model)
        {
            _service.UpdateProduct(model);
            return Ok();
        }

        public IActionResult DeleteProduct([FromQuery]long ProductId)
        {
            _service.DeleteProduct(ProductId);
            return Ok();
        }
        #endregion

        #region 訂單

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateOrder([FromBody]OrderCreateModel model)
        {
            _service.CreateOrder(model);
            return Ok();
        }
        #endregion

        #region 對外API
        [AllowAnonymous]
        [HttpGet]
        public IActionResult get_clothes_data(ClothSearchCondModel cond)
        {
            var result = _service.GetClothData(cond);
            return Json(result);
        }
        #endregion
    }
}
