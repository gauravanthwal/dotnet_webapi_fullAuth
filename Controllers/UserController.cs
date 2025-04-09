using FullAuth.Dto;
using FullAuth.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;   
        }


        [HttpGet]
        [Route("getall")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            ResponseDto res = new ResponseDto();

            res.Data = await _userRepository.GetAllUsers();

            return Ok(res);
        }
    }
}
