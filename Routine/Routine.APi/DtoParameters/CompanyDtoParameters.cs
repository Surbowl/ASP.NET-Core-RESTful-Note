namespace Routine.APi.DtoParameters
{
    public class CompanyDtoParameters
    {
        private const int MaxPageSize = 20; //翻页（视频P34-35）
        public string companyName { get; set; }
        public string SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1; //默认值为1
        public string OrderBy { get; set; } = "Name"; //默认用公司名字排序 //排序（视频P36-38）
        public string Fields { get; set; } //数据塑形（视频P39）
        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize ? MaxPageSize : value);
        }
    }
}
