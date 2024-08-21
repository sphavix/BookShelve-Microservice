using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShelve.Api.Domain.Data;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.Author;
using AutoMapper;
using BookShelve.Api.Static;

namespace BookShelve.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadAuthorDto>>> GetAuthors()
        {
            try
            {
                var authors = await _context.Authors.ToListAsync();
                var auhtorDtos = _mapper.Map<IEnumerable<ReadAuthorDto>>(authors);
                _logger.LogInformation(Messages.SuccessMessage);
                return Ok(auhtorDtos);
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error Performing the {nameof(GetAuthors)} method");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadAuthorDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    return NotFound();
                }
                var authorDto = _mapper.Map<ReadAuthorDto>(author);
                _logger.LogInformation(Messages.SuccessMessage);
                return Ok(authorDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error Performing the {nameof(GetAuthor)} method");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, UpdateAuthorDto authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            // Map Dto to Entity ==> source to destination
            _mapper.Map(authorDto, author);

            _context.Entry(author).State = EntityState.Modified;
            _logger.LogInformation(Messages.Success204Message);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {

                    return NotFound();

                }
                else
                {
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CreateAuthorDto>> PostAuthor(CreateAuthorDto authorDto)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDto);
                _context.Authors.Add(author);
                _logger.LogInformation(Messages.Success201Message);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch(Exception ex)
            {
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                _context.Authors.Remove(author);
                _logger.LogInformation(Messages.DeleteResourceMessage);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
