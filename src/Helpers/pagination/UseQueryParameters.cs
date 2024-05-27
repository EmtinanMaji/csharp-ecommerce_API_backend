public class QueryParameters{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string SearchKeyword { get; set; } = "";
    public string SortBy { get; set; } = "";
    public string SortOrder { get; set; } = "asc";

    public List<Guid>? SelectedCategories {get; set;} = new List<Guid>();
    public decimal? MinPrice {get; set;}
    public decimal? MaxPrice {get; set;}
}