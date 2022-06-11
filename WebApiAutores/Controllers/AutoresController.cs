using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utils;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var querable = context.Autores.AsQueryable();
            await HttpContext.InsertPaginationParamInHeader(querable);
            var autores = await querable.OrderBy(autor => autor.Name).Page(paginationDTO).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {

            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (autor == null) {
                return NotFound();
            }

            return Ok(mapper.Map<AutorDTOConLibros>(autor));
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string name)
        {
            var autores = await context.Autores.Where(x => x.Name.Contains(name)).ToListAsync();
            return Ok(mapper.Map<List<AutorDTO>>(autores));

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Name == autorCreacionDTO.Name);
            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Name}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorDTO, int id)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);
            if(!existeAutor)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Autores.AnyAsync(x => x.Id == id);

            if(!exist)
            {
                return NotFound("El autor no coincide con el id solicitado");
            }
            context.Remove(new Autor() { Id=id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
