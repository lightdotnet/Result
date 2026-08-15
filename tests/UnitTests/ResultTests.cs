#nullable disable
using System.Net;
using System.Text.Json;

namespace UnitTests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void Success_IsSuccess_True()
        {
            Result.Success().IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void Error_IsSuccess_False()
        {
            Result.Error().IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void All_Factories_Should_Return_Correct_Status()
        {
            Result.Success().Status.ShouldBe(ResultCode.Success);
            Result.BadRequest().Status.ShouldBe(ResultCode.BadRequest);
            Result.Unauthorized().Status.ShouldBe(ResultCode.Unauthorized);
            Result.Forbidden().Status.ShouldBe(ResultCode.Forbidden);
            Result.NotFound().Status.ShouldBe(ResultCode.NotFound);
            Result.Conflict().Status.ShouldBe(ResultCode.Conflict);
            Result.Error().Status.ShouldBe(ResultCode.Error);
        }

        [Test]
        public void Default_Constructor_Should_Be_Unknown()
        {
            var result = new Result();
            result.Status.ShouldBe(ResultCode.Unknown);
            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("unknown");
        }

        [Test]
        public void Code_Should_Return_Status_Name()
        {
            Result.Success().Code.ShouldBe("success");
            Result.Error().Code.ShouldBe("error");
            Result.NotFound("msg").Code.ShouldBe("not_found");
        }

        [Test]
        public void Code_Setter_Should_Sync_Status()
        {
            var result = new Result();
            result.Code = "not_found";
            result.Status.ShouldBe(ResultCode.NotFound);
            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("not_found");
            result.Code = "success";
            result.Status.ShouldBe(ResultCode.Success);
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void Code_Setter_Custom_Value_Should_Create_New_ResultCode()
        {
            var result = new Result();
            result.Code = "rate_limited";
            result.Status.Name.ShouldBe("rate_limited");
            result.Status.HttpStatus.ShouldBe(500);
            result.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void Code_Setter_Empty_Should_Not_Change_Status()
        {
            var result = Result.Success();
            result.Code = "";
            result.Status.ShouldBe(ResultCode.Success);
            result.Code = null;
            result.Status.ShouldBe(ResultCode.Success);
        }

        [Test]
        public void Message_Should_Default_Empty()
        {
            new Result().Message.ShouldBe("");
        }

        [Test]
        public void Message_Should_Be_Set()
        {
            Result.Success("ok").Message.ShouldBe("ok");
            Result.Error("fail").Message.ShouldBe("fail");
            Result<string>.NotFound("missing").Message.ShouldBe("missing");
        }

        [Test]
        public void RequestId_Should_Be_Lazy_And_Unique()
        {
            var r1 = Result.Success();
            var r2 = Result.Success();
            r1.RequestId.ShouldNotBeNullOrEmpty();
            r2.RequestId.ShouldNotBeNullOrEmpty();
            (r1.RequestId != r2.RequestId).ShouldBeTrue();
        }

        [Test]
        public void IsFailed_Should_Be_Opposite_Of_IsSuccess()
        {
            Result.Success().IsFailed().ShouldBeFalse();
            Result.Error().IsFailed().ShouldBeTrue();
            Result.NotFound().IsFailed().ShouldBeTrue();
        }

        [Test]
        public void IsFailed_Null_Result_Should_Return_True()
        {
            IResult nullResult = null;
            nullResult.IsFailed().ShouldBeTrue();
        }

        [Test]
        public void From_Should_Create_With_Custom_Code()
        {
            var custom = new ResultCode("custom", 418);
            var result = Result.From(custom, "I'm a teapot");
            result.Status.ShouldBe(custom);
            result.Code.ShouldBe("custom");
            result.Message.ShouldBe("I'm a teapot");
            result.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void From_Null_Should_Throw()
        {
            LightAssert.ShouldThrow<ArgumentNullException>(() => Result.From(null));
            LightAssert.ShouldThrow<ArgumentNullException>(() => Result<string>.From(null));
        }

        [Test]
        public void ResultOfT_From_Should_Create_With_Custom_Status()
        {
            var result = Result<int>.From(ResultCode.NotFound, "message");
            result.Status.ShouldBe(ResultCode.NotFound);
            result.Code.ShouldBe("not_found");
            result.Message.ShouldBe("message");
            result.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void From_Success_Code_Should_Return_Success_Result()
        {
            var result = Result.From(ResultCode.Success, "ok");
            result.IsSuccess.ShouldBeTrue();
            result.Code.ShouldBe("success");
        }

        [Test]
        public void ResultOfT_From_Success_Code_Should_Return_Success_Result()
        {
            var result = Result<int>.From(ResultCode.Success, "ok");
            result.IsSuccess.ShouldBeTrue();
            result.Code.ShouldBe("success");
        }

        [Test]
        public void From_Null_Message_Should_Normalize_To_Empty()
        {
            Result.From(ResultCode.Error, null).Message.ShouldBe("");
            Result<string>.From(ResultCode.Error, null).Message.ShouldBe("");
        }

        [Test]
        public void Success_With_Data_Null_Message_Should_Normalize_To_Empty()
        {
            Result<string>.Success("hello", null).Message.ShouldBe("");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Success_With_Data(int id)
        {
            var intResult = Result<int>.Success(id);
            intResult.Data.ShouldBe(id);
            intResult.IsSuccess.ShouldBeTrue();
            var strResult = Result<string>.Success($"ID-{id}");
            strResult.Data.ShouldBe($"ID-{id}");
            strResult.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void Success_Null_Should_Return_Error()
        {
            var result = Result<string>.Success(null);
            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("error");
            result.Data.ShouldBeNull();
            result.Message.ShouldBe("Data is null.");
        }

        [Test]
        public void Null_Data_Should_Return_Error_Not_NotFound()
        {
            var result = Result<string>.Success(null);
            result.Status.ShouldBe(ResultCode.Error);
            (result.Status != ResultCode.NotFound).ShouldBeTrue();
        }

        [Test]
        public void Error_Factories_Should_Have_Null_Data()
        {
            Result<string>.NotFound("x").Data.ShouldBeNull();
            Result<string>.BadRequest("x").Data.ShouldBeNull();
            Result<string>.Error("x").Data.ShouldBeNull();
            Result<string>.Conflict("x").Data.ShouldBeNull();
            Result<string>.Forbidden("x").Data.ShouldBeNull();
            Result<string>.Unauthorized("x").Data.ShouldBeNull();
        }

        [Test]
        public void Error_Factories_Should_Not_Be_Success()
        {
            Result<string>.NotFound().IsSuccess.ShouldBeFalse();
            Result<string>.BadRequest().IsSuccess.ShouldBeFalse();
            Result<string>.Error().IsSuccess.ShouldBeFalse();
            Result<string>.Conflict().IsSuccess.ShouldBeFalse();
            Result<string>.Forbidden().IsSuccess.ShouldBeFalse();
            Result<string>.Unauthorized().IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void Implicit_T_To_ResultT_Success()
        {
            string data = "hello";
            Result<string> result = data;
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe("hello");
            result.Code.ShouldBe("success");
        }

        [Test]
        public void Implicit_T_To_ResultT_Null_Returns_Error()
        {
            string nullData = null;
            Result<string> result = nullData;
            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("error");
            result.Data.ShouldBeNull();
        }

        [Test]
        public void Implicit_ResultT_To_T_Success_Returns_Data()
        {
            Result<string> result = Result<string>.Success("hello");
            string value = result;
            value.ShouldBe("hello");
        }

        [Test]
        public void Implicit_ResultT_To_T_Failed_Returns_Null()
        {
            Result<string> result = Result<string>.NotFound("not found");
            string value = result;
            value.ShouldBeNull();
        }

        [Test]
        public void Implicit_ResultT_To_T_ValueType_Failed_Returns_Default()
        {
            Result<int> result = Result<int>.Error("fail");
            int value = result;
            value.ShouldBe(0);
        }

        [Test]
        public void Implicit_ResultT_To_T_NullInstance_Returns_Default()
        {
            Result<string> result = null;
            string value = result;
            value.ShouldBeNull();
        }

        [Test]
        public void Implicit_ResultT_To_T_NullInstance_ValueType_Returns_Default()
        {
            Result<int> result = null;
            int value = result;
            value.ShouldBe(0);
        }

        [Test]
        public void Implicit_ResultT_To_Result_Should_Preserve()
        {
            var typed = Result<string>.Success("data", "msg");
            var requestId = typed.RequestId;
            Result simple = typed;
            simple.RequestId.ShouldBe(requestId);
            simple.Status.ShouldBe(ResultCode.Success);
            simple.Message.ShouldBe("msg");
        }

        [Test]
        public void Implicit_Result_To_ResultT_Should_Preserve()
        {
            var simple = Result.NotFound("msg");
            var requestId = simple.RequestId;
            Result<string> typed = simple;
            typed.RequestId.ShouldBe(requestId);
            typed.Status.ShouldBe(ResultCode.NotFound);
            typed.Message.ShouldBe("msg");
        }

        [Test]
        public void Implicit_ResultT_To_Result_NullInstance_Should_Throw()
        {
            LightAssert.ShouldThrow<NullReferenceException>(() =>
            {
                Result<string> typed = null;
                Result simple = typed;
            });
        }

        [Test]
        public void Implicit_Result_To_ResultT_NullInstance_Should_Throw()
        {
            LightAssert.ShouldThrow<NullReferenceException>(() =>
            {
                Result simple = null;
                Result<string> typed = simple;
            });
        }

        [Test]
        public void ToHttpStatusCode_Should_Map_All_BuiltIn()
        {
            Result.Success().ToHttpStatusCode().ShouldBe(HttpStatusCode.OK);
            Result.BadRequest().ToHttpStatusCode().ShouldBe(HttpStatusCode.BadRequest);
            Result.Unauthorized().ToHttpStatusCode().ShouldBe(HttpStatusCode.Unauthorized);
            Result.Forbidden().ToHttpStatusCode().ShouldBe(HttpStatusCode.Forbidden);
            Result.NotFound().ToHttpStatusCode().ShouldBe(HttpStatusCode.NotFound);
            Result.Conflict().ToHttpStatusCode().ShouldBe(HttpStatusCode.Conflict);
            Result.Error().ToHttpStatusCode().ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Test]
        public void ToHttpStatusCode_Custom_Code()
        {
            var custom = new ResultCode("rate_limited", 429);
            Result.From(custom).ToHttpStatusCode().ShouldBe((HttpStatusCode)429);
        }

        [Test]
        public void ToHttpStatusCode_Unknown_Returns_500()
        {
            new Result().ToHttpStatusCode().ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Test]
        public void Status_Null_Should_Fallback_To_Unknown_Defaults()
        {
            var result = Result.Success();
            result.Status = null;
            result.Code.ShouldBe("unknown");
            result.IsSuccess.ShouldBeFalse();
            result.ToHttpStatusCode().ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Test]
        public void ToHttpStatusCode_NonResultBase_IResult_Should_Resolve_Via_Code()
        {
            IResult custom = new PlainResult("not_found");
            custom.ToHttpStatusCode().ShouldBe(HttpStatusCode.NotFound);
        }

        private sealed class PlainResult : IResult
        {
            public PlainResult(string code) => Code = code;

            public string RequestId => "id";
            public string Code { get; }
            public bool IsSuccess => false;
            public string Message => "";
        }

        [Test]
        public void Serialize_Should_Not_Include_Status_Field()
        {
            var result = Result.Success("ok");
            var json = JsonSerializer.Serialize(result);
            Assert.That(json, Does.Contain(@"""Code"""));
            Assert.That(json, Does.Contain(@"""IsSuccess"""));
            Assert.That(json, Does.Contain(@"""Message"""));
            Assert.That(json, Does.Not.Contain(@"""Status"""));
        }

        [Test]
        public void Deserialize_Should_Restore_Via_Code_Setter()
        {
            var original = Result.NotFound("not here");
            var json = JsonSerializer.Serialize(original);
            var restored = JsonSerializer.Deserialize<Result>(json);
            restored.Code.ShouldBe("not_found");
            restored.IsSuccess.ShouldBeFalse();
            restored.Status.ShouldBe(ResultCode.NotFound);
            restored.Message.ShouldBe("not here");
        }

        [Test]
        public void Deserialize_ResultT_Should_Restore()
        {
            var original = Result<string>.Success("hello", "msg");
            var json = JsonSerializer.Serialize(original);
            var restored = JsonSerializer.Deserialize<Result<string>>(json);
            restored.Code.ShouldBe("success");
            restored.IsSuccess.ShouldBeTrue();
            restored.Data.ShouldBe("hello");
            restored.Message.ShouldBe("msg");
        }
    }
}
