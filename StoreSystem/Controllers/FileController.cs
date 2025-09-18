using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Services;

namespace StoreSystem.Controllers
{
    public class FileController : Controller
    {
        private readonly FileService _service;

        public FileController(FileService service) { 
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Image([FromQuery]long id)
        {
            (var file,var contentType) = _service.GetImage(id);
            return File(file,contentType);
        }
    }
}
