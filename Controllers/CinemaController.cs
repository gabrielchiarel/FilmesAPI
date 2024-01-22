using AutoMapper;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public CinemaController(
        FilmeContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um cinema ao banco de dados
    /// </summary>
    /// <param name="cinemaDto">Objeto com os campos necessários para criação de um cinema</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AddFilme([FromBody] CreateCinemaDto cinemaDto)
    {
        Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
        _context.Cinemas.Add(cinema);
        _context.SaveChanges();
        return CreatedAtAction(nameof(PegarCinemaPorId),
            new { id = cinema.Id },
            cinema);
    }

    [HttpGet]
    public IEnumerable<ReadCinemaDto> PegarCinemas(
        [FromQuery] int? enderecoId = null)
    {
        if (enderecoId == null)
        {
            return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.ToList());
        }

        return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.FromSqlRaw(
            $@"SELECT
            {nameof(Cinema.Id)}, 
            {nameof(Cinema.Nome)},
            {nameof(Cinema.EnderecoId)}
            FROM cinemas
            WHERE cinemas.{nameof(Cinema.EnderecoId)} = {enderecoId}").ToList());
    }

    [HttpGet("{id}")]
    public IActionResult PegarCinemaPorId(int id)
    {
        var cinema = _context.Cinemas.SingleOrDefault(x => x.Id == id);
        if (cinema == null)
        {
            return NotFound();
        }
        var cinemaDto = _mapper.Map<ReadCinemaDto>(cinema);

        return Ok(cinemaDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCinema(int id,
        [FromBody] UpdateCinemaDto cinemaDto)
    {
        var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
        if (cinema == null)
        {
            return NotFound();
        }

        _mapper.Map(cinemaDto, cinema);

        _context.Cinemas.Update(cinema);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateCinemaPatch(int id,
        JsonPatchDocument<UpdateCinemaDto> patch)
    {
        var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
        if (cinema == null)
        {
            return NotFound();
        }

        var cinemaToUpdate = _mapper.Map<UpdateCinemaDto>(cinema);
        patch.ApplyTo(cinemaToUpdate, ModelState);

        if (!TryValidateModel(cinemaToUpdate))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(cinemaToUpdate, cinema);

        _context.Cinemas.Update(cinema);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCinema(int id)
    {
        var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
        if (cinema == null)
        {
            return NotFound();
        }

        _context.Cinemas.Remove(cinema);
        _context.SaveChanges();

        return NoContent();
    }
}
