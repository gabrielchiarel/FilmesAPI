using AutoMapper;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SessaoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public SessaoController(
        FilmeContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um sessao ao banco de dados
    /// </summary>
    /// <param name="sessaoDto">Objeto com os campos necessários para criação de um sessao</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AddFilme([FromBody] CreateSessaoDto sessaoDto)
    {
        Sessao sessao = _mapper.Map<Sessao>(sessaoDto);
        _context.Sessoes.Add(sessao);
        _context.SaveChanges();
        return CreatedAtAction(nameof(PegarSessaoPorCinemaIdEFilmeId),
            new { filmeId = sessao.FilmeId, cinemaId = sessao.CinemaId },
            sessao);
    }

    [HttpGet]
    public IEnumerable<ReadSessaoDto> PegarSessoes([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var sessoes = _context.Sessoes.Skip(skip).Take(take).ToList();
        var sessoesDto = _mapper.Map<List<ReadSessaoDto>>(sessoes);
        return sessoesDto;
    }

    [HttpGet("{filmeId}/{cinemaId}")]
    public IActionResult PegarSessaoPorCinemaIdEFilmeId(int filmeId, int cinemaId)
    {
        var sessao = _context.Sessoes.SingleOrDefault(x => x.CinemaId == cinemaId && x.FilmeId == filmeId);
        if (sessao == null)
        {
            return NotFound();
        }
        var sessaoDto = _mapper.Map<ReadSessaoDto>(sessao);

        return Ok(sessaoDto);
    }

    //[HttpPut("{id}")]
    //public IActionResult UpdateSessao(int id,
    //    [FromBody] UpdateSessaoDto sessaoDto)
    //{
    //    var sessao = _context.Sessoes.FirstOrDefault(x => x.Id == id);
    //    if (sessao == null)
    //    {
    //        return NotFound();
    //    }

    //    _mapper.Map(sessaoDto, sessao);

    //    _context.Sessoes.Update(sessao);
    //    _context.SaveChanges();

    //    return NoContent();
    //}

    //[HttpPatch("{id}")]
    //public IActionResult UpdateSessaoPatch(int id,
    //    JsonPatchDocument<UpdateSessaoDto> patch)
    //{
    //    var sessao = _context.Sessoes.FirstOrDefault(x => x.Id == id);
    //    if (sessao == null)
    //    {
    //        return NotFound();
    //    }

    //    var sessaoToUpdate = _mapper.Map<UpdateSessaoDto>(sessao);
    //    patch.ApplyTo(sessaoToUpdate, ModelState);

    //    if (!TryValidateModel(sessaoToUpdate))
    //    {
    //        return ValidationProblem(ModelState);
    //    }

    //    _mapper.Map(sessaoToUpdate, sessao);

    //    _context.Sessoes.Update(sessao);
    //    _context.SaveChanges();

    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public IActionResult DeleteSessao(int id)
    //{
    //    var sessao = _context.Sessoes.FirstOrDefault(x => x.Id == id);
    //    if (sessao == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Sessoes.Remove(sessao);
    //    _context.SaveChanges();

    //    return NoContent();
    //}
}
