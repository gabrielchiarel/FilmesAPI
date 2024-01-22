using AutoMapper;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EnderecoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public EnderecoController(
        FilmeContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um endereco ao banco de dados
    /// </summary>
    /// <param name="enderecoDto">Objeto com os campos necessários para criação de um endereco</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AddFilme([FromBody] CreateEnderecoDto enderecoDto)
    {
        Endereco endereco = _mapper.Map<Endereco>(enderecoDto);
        _context.Enderecos.Add(endereco);
        _context.SaveChanges();
        return CreatedAtAction(nameof(PegarEnderecoPorId),
            new { id = endereco.Id },
            endereco);
    }

    [HttpGet]
    public IEnumerable<ReadEnderecoDto> PegarEnderecos([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var enderecos = _context.Enderecos.Skip(skip).Take(take).ToList();
        var enderecosDto = _mapper.Map<List<ReadEnderecoDto>>(enderecos);
        return enderecosDto;
    }

    [HttpGet("{id}")]
    public IActionResult PegarEnderecoPorId(int id)
    {
        var endereco = _context.Enderecos.SingleOrDefault(x => x.Id == id);
        if (endereco == null)
        {
            return NotFound();
        }
        var enderecoDto = _mapper.Map<ReadEnderecoDto>(endereco);

        return Ok(enderecoDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEndereco(int id,
        [FromBody] UpdateEnderecoDto enderecoDto)
    {
        var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
        if (endereco == null)
        {
            return NotFound();
        }

        _mapper.Map(enderecoDto, endereco);

        _context.Enderecos.Update(endereco);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateEnderecoPatch(int id,
        JsonPatchDocument<UpdateEnderecoDto> patch)
    {
        var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
        if (endereco == null)
        {
            return NotFound();
        }

        var enderecoToUpdate = _mapper.Map<UpdateEnderecoDto>(endereco);
        patch.ApplyTo(enderecoToUpdate, ModelState);

        if (!TryValidateModel(enderecoToUpdate))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(enderecoToUpdate, endereco);

        _context.Enderecos.Update(endereco);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEndereco(int id)
    {
        var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
        if (endereco == null)
        {
            return NotFound();
        }

        _context.Enderecos.Remove(endereco);
        _context.SaveChanges();

        return NoContent();
    }
}
