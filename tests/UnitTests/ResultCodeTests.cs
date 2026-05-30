#nullable disable
namespace UnitTests
{
    [TestFixture]
    public class ResultCodeTests
    {
        [Test]
        public void BuiltIn_Codes_Should_Have_Correct_Properties()
        {
            ResultCode.Unknown.Name.ShouldBe("unknown");
            ResultCode.Unknown.HttpStatus.ShouldBe(500);
            ResultCode.Unknown.IsSuccess.ShouldBeFalse();

            ResultCode.Success.Name.ShouldBe("success");
            ResultCode.Success.HttpStatus.ShouldBe(200);
            ResultCode.Success.IsSuccess.ShouldBeTrue();

            ResultCode.BadRequest.Name.ShouldBe("bad_request");
            ResultCode.BadRequest.HttpStatus.ShouldBe(400);
            ResultCode.BadRequest.IsSuccess.ShouldBeFalse();

            ResultCode.Unauthorized.Name.ShouldBe("unauthorized");
            ResultCode.Unauthorized.HttpStatus.ShouldBe(401);
            ResultCode.Unauthorized.IsSuccess.ShouldBeFalse();

            ResultCode.Forbidden.Name.ShouldBe("forbidden");
            ResultCode.Forbidden.HttpStatus.ShouldBe(403);
            ResultCode.Forbidden.IsSuccess.ShouldBeFalse();

            ResultCode.NotFound.Name.ShouldBe("not_found");
            ResultCode.NotFound.HttpStatus.ShouldBe(404);
            ResultCode.NotFound.IsSuccess.ShouldBeFalse();

            ResultCode.Conflict.Name.ShouldBe("conflict");
            ResultCode.Conflict.HttpStatus.ShouldBe(409);
            ResultCode.Conflict.IsSuccess.ShouldBeFalse();

            ResultCode.Error.Name.ShouldBe("error");
            ResultCode.Error.HttpStatus.ShouldBe(500);
            ResultCode.Error.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void Equality_Should_Be_Based_On_Name()
        {
            var codeA = new ResultCode("test", 200, true);
            var codeB = new ResultCode("test", 500, false);
            var codeC = new ResultCode("other", 200, true);

            (codeA == codeB).ShouldBeTrue();
            (codeA != codeC).ShouldBeTrue();
            codeA.Equals(codeB).ShouldBeTrue();
            codeA.GetHashCode().ShouldBe(codeB.GetHashCode());
        }

        [Test]
        public void Null_Equality_Should_Be_Safe()
        {
            ResultCode code = ResultCode.Success;
            ResultCode nullCode = null;

            (code == null).ShouldBeFalse();
            (null == code).ShouldBeFalse();
            (code != null).ShouldBeTrue();
            (nullCode == null).ShouldBeTrue();
            (null == nullCode).ShouldBeTrue();
        }

        [Test]
        public void ToString_Should_Return_Name()
        {
            ResultCode.Success.ToString().ShouldBe("success");
            ResultCode.NotFound.ToString().ShouldBe("not_found");
        }

        [Test]
        public void Implicit_String_Should_Return_Name()
        {
            string code = ResultCode.Success;
            code.ShouldBe("success");
        }

        [Test]
        public void Implicit_String_Null_Should_Return_Null()
        {
            ResultCode nullCode = null;
            string s = nullCode;
            s.ShouldBeNull();
        }

        [Test]
        public void Constructor_Null_Name_Should_Throw()
        {
            LightAssert.ShouldThrow<ArgumentNullException>(() =>
            {
                new ResultCode(null);
            });
        }

        [Test]
        public void Custom_Code_Should_Work()
        {
            var rateLimited = new ResultCode("rate_limited", 429);
            rateLimited.Name.ShouldBe("rate_limited");
            rateLimited.HttpStatus.ShouldBe(429);
            rateLimited.IsSuccess.ShouldBeFalse();

            var partial = new ResultCode("partial_success", 207, true);
            partial.Name.ShouldBe("partial_success");
            partial.HttpStatus.ShouldBe(207);
            partial.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void FromName_Should_Return_BuiltIn_Singleton()
        {
            Assert.That(ResultCode.FromName("success"), Is.SameAs(ResultCode.Success));
            Assert.That(ResultCode.FromName("bad_request"), Is.SameAs(ResultCode.BadRequest));
            Assert.That(ResultCode.FromName("unauthorized"), Is.SameAs(ResultCode.Unauthorized));
            Assert.That(ResultCode.FromName("forbidden"), Is.SameAs(ResultCode.Forbidden));
            Assert.That(ResultCode.FromName("not_found"), Is.SameAs(ResultCode.NotFound));
            Assert.That(ResultCode.FromName("conflict"), Is.SameAs(ResultCode.Conflict));
            Assert.That(ResultCode.FromName("error"), Is.SameAs(ResultCode.Error));
            Assert.That(ResultCode.FromName("unknown"), Is.SameAs(ResultCode.Unknown));
        }

        [Test]
        public void FromName_Null_Or_Empty_Should_Return_Unknown()
        {
            Assert.That(ResultCode.FromName(null), Is.SameAs(ResultCode.Unknown));
            Assert.That(ResultCode.FromName(""), Is.SameAs(ResultCode.Unknown));
        }

        [Test]
        public void FromName_Custom_Should_Return_New_Instance()
        {
            var custom = ResultCode.FromName("rate_limited");
            custom.Name.ShouldBe("rate_limited");
            custom.HttpStatus.ShouldBe(500);
        }
    }
}
