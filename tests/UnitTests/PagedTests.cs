using System.Text.Json;

namespace UnitTests
{
    public class PagedTests
    {
        private readonly int totalRecords = 10;
        private readonly int pageSize = 5;

        private readonly IEnumerable<int> listValues;

        public PagedTests()
        {
            var list = new List<int>();

            for (int i = 1; i <= totalRecords; i++)
            {
                list.Add(i);
            }

            listValues = list;
        }

        [Fact]
        public void Should_Return_Correct_PageInfo()
        {
            var pagedResult = listValues.ToPagedResult(1, pageSize);

            var pagedData = pagedResult.Data;

            pagedData.PageSize.ShouldBe(pageSize);
            pagedData.TotalRecords.ShouldBe(totalRecords);
        }

        [Fact]
        public void Should_Return_Correct_PageData()
        {
            var pagedResult = listValues.ToPagedResult(1, pageSize);

            var recordsPerPage = pagedResult.Data.Records.Count();

            recordsPerPage.ShouldBe(pageSize);
        }

        [Fact]
        public void Should_Serialize_Correct_PagedData()
        {
            var pagedResult = listValues.ToPagedResult(1, pageSize);

            var json = JsonSerializer.Serialize(pagedResult);

            var deserializeData = JsonSerializer.Deserialize<PagedResult<int>>(json);

            deserializeData!.Data.Page.ShouldBe(1);
            deserializeData!.Data.PageSize.ShouldBe(pageSize);
        }
    }
}