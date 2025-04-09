namespace FullAuth.Dto
{
    public class ResponseDto
    {
        public string Message { get; set; } = "Success";
        public bool IsSuccess { get; set; } = true;
        public object? Data { get; set; } = null;
    }
}
