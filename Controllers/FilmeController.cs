using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(
        FilmeContext context, 
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AddFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(PegarFilmePorId), 
            new { id = filme.Id },
            filme);
    }

    [HttpGet]
    public IEnumerable<ReadFilmeDto> PegarFilmes(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 10,
        [FromQuery] string? titulo = null)
    {
        List<Filme> filmes;
        if (titulo == null)
        {
            filmes = _context.Filmes
                .Skip(skip)
                .Take(take)
                .ToList();
        } 
        else
        {
            filmes = _context.Filmes
                .Where(filme => filme.Titulo.Contains(titulo))
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        var filmesDto = _mapper.Map<List<ReadFilmeDto>>(filmes);
        return filmesDto;
    }

    [HttpGet("{id}")]
    public IActionResult PegarFilmePorId(int id)
    {
        var filme = _context.Filmes.SingleOrDefault(x => x.Id == id);
        if (filme == null)
        {
            return NotFound();
        }
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);

        return Ok(filmeDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateFilme(int id, 
        [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null) 
        { 
            return NotFound(); 
        }

        _mapper.Map(filmeDto, filme);

        _context.Filmes.Update(filme);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateFilmePatch(int id,
        JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null)
        {
            return NotFound();
        }

        var filmeToUpdate = _mapper.Map<UpdateFilmeDto>(filme);
        patch.ApplyTo(filmeToUpdate, ModelState);

        if (!TryValidateModel(filmeToUpdate))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeToUpdate, filme);

        _context.Filmes.Update(filme);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null)
        {
            return NotFound();
        }

        _context.Filmes.Remove(filme);
        _context.SaveChanges();

        return NoContent();
    }
}
