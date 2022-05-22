using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController: ControllerBase
    {
        [HttpGet]
		[ResponseCache(Duration = 1200,VaryByQueryKeys = new[] { "fileName"})]
        public ActionResult GetFile([FromQuery] string fileName)
		{
            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{rootPath}/PrivateFiles/{fileName}";

            //Sprawdzanie czy plik istnieje

           var fileExists = System.IO.File.Exists(filePath);
            if(!fileExists)
			{
                return NotFound();
			}
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);
            
                var fileContent =System.IO.File.ReadAllBytes(filePath);

            return File(fileContent, contentType, fileName);


            //return Ok(fileContent); lub
           
		}
        [HttpPost]
        public ActionResult Upload([FromForm]IFormFile file)
		{
            if(file != null && file.Length > 0)
			{
                var rootPath = Directory.GetCurrentDirectory();
                var fileName = file.FileName;
                var filePath = $"{rootPath}/PrivateFiles/{fileName}";
                //Zapisywanie pliku na serwer
                using(var stream = new FileStream(filePath, FileMode.Create))
				{
                    file.CopyTo(stream);
				}
                return Ok();
			}
            return BadRequest();
		}

    }
}
