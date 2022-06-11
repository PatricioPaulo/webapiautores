using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public ActionResult<IEnumerable<DatosHATEOSDTO>> Get()
        {
            var datosHateos = new List<DatosHATEOSDTO>();

            datosHateos.Add(new DatosHATEOSDTO(link: Url.Link("GetRoot", null),
                description: "self", method: "GET"));
            
            return datosHateos;
        }
    }
}
