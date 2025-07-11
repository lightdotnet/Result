using Light.Contracts;
using Light.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResultController : ApiControllerBase
    {
        private readonly List<int> _list;

        public ResultController()
        {
            var list = new List<int>();
            for (int i = 0; i < 20; i++)
                list.Add(i);

            _list = list;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //var res = Result.NotFound("Error message");
            //var res = new Result { Code = "ABC", Message = "" };

            var res = Result.Success();

            return Ok(ResultService.GetResult());
        }

        [HttpGet("object")]
        public IActionResult GetObject()
        {
            var data = new { Id = 1, Name = "Test 1" };

            return Ok(data);
        }

        [HttpGet("success")]
        public IActionResult GetSuccess()
        {
            var res = Result.Success();
            return Ok(res);
        }

        [HttpGet("error")]
        public IActionResult GetError()
        {
            var res = Result.Error();
            return Ok(res);
        }

        [HttpGet("find")]
        public IActionResult FindValue()
        {
            var model = _list.Select(s => s.ToString()).FirstOrDefault(x => x == "A");
            return Ok(model);
        }


        [HttpGet("paged")]
        public IActionResult GetPaged(int page = 1, int pageSize = 10)
        {
            return Ok(_list.ToPagedResult(page, pageSize));
        }

        [HttpGet("mapper-paged")]
        public IActionResult GetMapperPaged(int page, int size)
        {
            var paged = _list.ToPagedResult(page, size);
            var result = paged.Adapt<PagedResult<int>>();
            return Ok(result);
        }

        [HttpGet("deserialize-paged")]
        public IActionResult DeserializePaged(int page, int size)
        {
            var list = _list.ToPagedResult(page, size);
            var json = JsonSerializer.Serialize(list);

            var result = JsonSerializer.Deserialize<PagedResult<int>>(json);

            return Ok(result);
        }

        [HttpGet("errors")]
        public IActionResult Error()
        {
            var error = Result.Error("Error1");
            var errorT = Result<string>.Error("Error1");

            return Ok(new { error, errorT });
        }
    }

    public static class ResultService
    {
        public static Result GetResult()
        {
            //return Result<Guid>.Success(Guid.NewGuid());
            return Result<Guid>.Error("Error when get ID");
        }
    }
}
