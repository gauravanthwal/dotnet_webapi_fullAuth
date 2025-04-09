using BCrypt.Net;
using FullAuth.Dto;
using FullAuth.Entities;
using FullAuth.Helper;
using FullAuth.Repository;
using FullAuth.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;

        public AuthController(IUserRepository userRepository, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }



        /*************************************************************/
        /******************** LOGIN USER ****************************/
        /*************************************************************/
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginUserReqDto req)
        {
            User? user = await _userRepository.GetUserByEmail(req.Email);
            ResponseDto res = new ResponseDto();

            if (user == null) {
                res.IsSuccess = false;
                res.Message = "Invalid Credential!!";
                return BadRequest(res);
            }

            if(!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                res.IsSuccess = false;
                res.Message = "Invalid Credential!";
                return BadRequest(res);
            }

            var RefreshToken = _jwtHelper.GenerateRefreshToken();
            var AccessToken = _jwtHelper.GenerateJwtToken(user);

            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpire = DateTime.Now.AddDays(2);

            await _userRepository.UpdateUser(user);

            LoginUserResDto userDetails = new LoginUserResDto()
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken
            };

            res.Data = userDetails;
            return Ok(res);
        }


        /*************************************************************/
        /******************** REGISTER USER ****************************/
        /*************************************************************/
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] RegisterUserDto req)
        {
            ResponseDto res = new ResponseDto();
            User? user = await _userRepository.GetUserByEmail(req.Email);

            if (user != null) {
                res.IsSuccess = false;
                res.Message = "User already registered!";
                return BadRequest(res);
            }

            User newUser = new User()
            {
                UserName = req.UserName,
                Email = req.Email,
                Address = req.Address,
                Role = UserRoles.USER.ToString(),
                RefreshToken = "",
                RefreshTokenExpire = DateTime.Now.AddDays(2),
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            bool result = await _userRepository.AddUser(newUser);

            if (!result) {
                res.IsSuccess = false;
                res.Message = $"Internal Server Error";
                return BadRequest(res);
            }

            res.Message = "User registered successfully";
            return Ok(res);
        }


        /*************************************************************/
        /******************** REFRESH TOKEN ****************************/
        /*************************************************************/
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<ResponseDto>> RefreshToken([FromBody] RefreshTokenDto req)
        {
            ResponseDto res = new ResponseDto();

            if (req.RefreshToken == null || req.AccessToken == null) {
                res.IsSuccess = false;
                res.Message = "Invalid Request";
                return BadRequest(res);
            }

            var RefreshToken = req.RefreshToken;
            var AccessToken = req.AccessToken;

            var principal = _jwtHelper.GetPrincipalFromExpiredToken(AccessToken);
            var email = principal.Identity.Name;


            User? user = await _userRepository.GetUserByEmail(email);

            if (user == null) {
                res.IsSuccess = false;
                res.Message = "Invalid Request";
                return BadRequest(res);
            }

            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var accessToken = _jwtHelper.GenerateJwtToken(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpire = DateTime.Now.AddDays(2);

            await _userRepository.UpdateUser(user);

            LoginUserResDto userDetails = new LoginUserResDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            res.Data = userDetails;
            return Ok(res);
        }


        /*************************************************************/
        /******************** REGISTER ADMIN ****************************/
        /*************************************************************/
        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult<ResponseDto>> RegisterAdmin([FromBody] RegisterUserDto req)
        {
            ResponseDto res = new ResponseDto();
            User? user = await _userRepository.GetUserByEmail(req.Email);

            if (user != null)
            {
                res.IsSuccess = false;
                res.Message = "User already registered!";
                return BadRequest(res);
            }

            User newUser = new User()
            {
                UserName = req.UserName,
                Email = req.Email,
                Address = req.Address,
                Role = UserRoles.ADMIN.ToString(),
                RefreshToken = "",
                RefreshTokenExpire = DateTime.Now.AddDays(2),
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            bool result = await _userRepository.AddUser(newUser);

            if (!result)
            {
                res.IsSuccess = false;
                res.Message = $"Internal Server Error";
                return BadRequest(res);
            }

            res.Message = "User registered successfully";
            return Ok(res);
        }



        /*************************************************************/
        /******************** LOGOUT USER ****************************/
        /*************************************************************/
        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<ActionResult<ResponseDto>> Revoke()
        {
            ResponseDto res = new ResponseDto();


            var email = User.Identity.Name;
            User? user = await _userRepository.GetUserByEmail(email);

            if (user == null) {
                res.IsSuccess = false;
                res.Message = "Invalid Request";
                return BadRequest(res);
            }

            user.RefreshToken = "";

            await _userRepository.UpdateUser(user);

            res.Message = "User successfully logout";

            return Ok(res);
        }

    }
}
