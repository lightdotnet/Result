#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class PagedTests
    {
        private int _totalRecords;
        private int _pageSize;
        private List<int> _list;

        [SetUp]
        public void Setup()
        {
            _totalRecords = 10;
            _pageSize = 5;
            _list = Enumerable.Range(1, _totalRecords).ToList();
        }

        [Test]
        public void Should_Return_Correct_PageInfo()
        {
            var result = _list.ToPagedResult(1, _pageSize);

            result.IsSuccess.ShouldBeTrue();
            result.Data.PageNumber.ShouldBe(1);
            result.Data.PageSize.ShouldBe(_pageSize);
            result.Data.TotalRecords.ShouldBe(_totalRecords);
        }

        [Test]
        public void Should_Return_Correct_PageData()
        {
            var result = _list.ToPagedResult(1, _pageSize);
            result.Data.Records.Count().ShouldBe(_pageSize);
        }

        [Test]
        public void Should_Serialize_Correct_PagedData()
        {
            var paged = _list.ToPaged(1, _pageSize);

            var json = JsonSerializer.Serialize(paged);
            var deserialized = JsonSerializer.Deserialize<Paged<int>>(json);

            deserialized.PageNumber.ShouldBe(1);
            deserialized.PageSize.ShouldBe(_pageSize);
            deserialized.TotalRecords.ShouldBe(_totalRecords);
            deserialized.Records.Count().ShouldBe(_pageSize);
        }

        [Test]
        public void TotalPages_Should_Calculate_Correctly()
        {
            new Paged { TotalRecords = 10, PageSize = 5 }.TotalPages.ShouldBe(2);
            new Paged { TotalRecords = 11, PageSize = 5 }.TotalPages.ShouldBe(3);
            new Paged { TotalRecords = 0, PageSize = 5 }.TotalPages.ShouldBe(0);
            new Paged { TotalRecords = 10, PageSize = 0 }.TotalPages.ShouldBe(0);
            new Paged { TotalRecords = 1, PageSize = 10 }.TotalPages.ShouldBe(1);
        }

        [Test]
        public void HasPreviousPage_And_HasNextPage()
        {
            var page1 = _list.ToPaged(1, _pageSize);
            page1.HasPreviousPage.ShouldBeFalse();
            page1.HasNextPage.ShouldBeTrue();

            var page2 = _list.ToPaged(2, _pageSize);
            page2.HasPreviousPage.ShouldBeTrue();
            page2.HasNextPage.ShouldBeFalse();
        }

        [Test]
        public void Records_Should_Default_To_Empty()
        {
            var paged = new Paged<int>();
            paged.Records.ShouldNotBeNull();
            paged.Records.Count().ShouldBe(0);
        }

        [Test]
        public void Paged_Constructor_Null_Data_Should_Fallback_Empty()
        {
            var paged = new Paged<int>(null, 1, 10, 0);
            paged.Records.ShouldNotBeNull();
            paged.Records.Count().ShouldBe(0);
        }

        [Test]
        public void ToPagedResult_Null_List_Should_Return_Error()
        {
            IEnumerable<int> nullList = null;
            var result = nullList.ToPagedResult();

            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("error");
        }

        [Test]
        public void ToPagedResult_Null_Page_Should_Throw()
        {
            LightAssert.ShouldThrow<ArgumentNullException>(() =>
            {
                _list.ToPagedResult((IPage)null);
            });
        }

        [Test]
        public void ToPagedResult_Should_Clamp_Invalid_Values()
        {
            var result = _list.ToPagedResult(0, -1);

            result.Data.PageNumber.ShouldBe(1);
            result.Data.PageSize.ShouldBe(10);
        }

        [Test]
        public void ToPagedResult_With_IPage_Should_Work()
        {
            IPage page = new Paged { PageNumber = 2, PageSize = 3 };
            var result = _list.ToPagedResult(page);

            result.IsSuccess.ShouldBeTrue();
            result.Data.PageNumber.ShouldBe(2);
            result.Data.PageSize.ShouldBe(3);
            result.Data.TotalRecords.ShouldBe(_totalRecords);
            result.Data.Records.Count().ShouldBe(3);
        }

        [Test]
        public void ToPaged_Null_List_Should_Throw()
        {
            LightAssert.ShouldThrow<ArgumentNullException>(() =>
            {
                ((IEnumerable<int>)null).ToPaged();
            });
        }

        [Test]
        public void ToPaged_Null_Page_Should_Throw()
        {
            LightAssert.ShouldThrow<ArgumentNullException>(() =>
            {
                _list.ToPaged((IPage)null);
            });
        }

        [Test]
        public void ToPaged_With_IPage_Should_Work()
        {
            IPage page = new Paged { PageNumber = 1, PageSize = 4 };
            var paged = _list.ToPaged(page);

            paged.PageNumber.ShouldBe(1);
            paged.PageSize.ShouldBe(4);
            paged.TotalRecords.ShouldBe(_totalRecords);
            paged.Records.Count().ShouldBe(4);
        }

        [Test]
        public void PagedResult_Null_Data_Should_Return_Error()
        {
            var result = new PagedResult<int>((Paged<int>)null);

            result.IsSuccess.ShouldBeFalse();
            result.Code.ShouldBe("error");
            result.Data.ShouldBeNull();
        }

        [Test]
        public void PagedResult_Implicit_Should_Return_Data()
        {
            var result = _list.ToPagedResult(1, _pageSize);
            Paged<int> paged = result;

            paged.ShouldNotBeNull();
            paged.PageNumber.ShouldBe(1);
            paged.Records.Count().ShouldBe(_pageSize);
        }

        [Test]
        public void PagedResult_Implicit_Null_Should_Return_Null()
        {
            PagedResult<int> nullResult = null;
            Paged<int> paged = nullResult;

            paged.ShouldBeNull();
        }

        [Test]
        public void PagedResult_Implicit_NullData_Should_Return_Null()
        {
            var result = new PagedResult<int>();
            Paged<int> paged = result;

            paged.ShouldBeNull();
        }
    }
}
